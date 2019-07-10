using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Web;
using System;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Results;
using ServiceStack.Validation;

namespace ServiceStack
{
    public class InProcessServiceGateway : IServiceGateway, IServiceGatewayAsync
    {
        public IRequest Request { get; }

        public InProcessServiceGateway(IRequest req)
        {
            this.Request = req;
        }

        private string SetVerb(object reqeustDto)
        {
            var hold = this.Request.GetItem(Keywords.InvokeVerb) as string;
            if (reqeustDto is IVerb)
            {
                if (reqeustDto is IGet)
                    this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Get);
                if (reqeustDto is IPost)
                    this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Post);
                if (reqeustDto is IPut)
                    this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Put);
                if (reqeustDto is IDelete)
                    this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Delete);
                if (reqeustDto is IPatch)
                    this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Patch);
                if (reqeustDto is IOptions)
                    this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Options);
            }
            return hold;
        }

        private void ResetVerb(string verb)
        {
            if (verb == null)
                this.Request.Items.Remove(Keywords.InvokeVerb);
            else
                this.Request.SetItem(Keywords.InvokeVerb, verb);
        }

        private TResponse ExecSync<TResponse>(object request)
        {
            foreach (var filter in HostContext.AppHost.GatewayRequestFiltersArray)
            {
                filter(this.Request, request);
                if (this.Request.Response.IsClosed)
                    return default(TResponse);
            }
            foreach (var filter in HostContext.AppHost.GatewayRequestFiltersAsyncArray)
            {
                filter(this.Request, request).Wait();
                if (this.Request.Response.IsClosed)
                    return default(TResponse);
            }

            ExecValidators(request).Wait();

            var response = HostContext.ServiceController.Execute(request, this.Request);
            if (response is Task responseTask)
                response = responseTask.GetResult();

            if (response is Task[] batchResponseTasks)
            {
                Task.WaitAll(batchResponseTasks);
                var to = new object[batchResponseTasks.Length];
                for (var i = 0; i < batchResponseTasks.Length; i++)
                {
                    to[i] = batchResponseTasks[i].GetResult();
                }
                response = to.ConvertTo<TResponse>();
            }

            var responseDto = ConvertToResponse<TResponse>(response);

            foreach (var filter in HostContext.AppHost.GatewayResponseFiltersArray)
            {
                filter(this.Request, responseDto);
                if (this.Request.Response.IsClosed)
                    return default(TResponse);
            }
            foreach (var filter in HostContext.AppHost.GatewayResponseFiltersAsyncArray)
            {
                filter(this.Request, responseDto).Wait();
                if (this.Request.Response.IsClosed)
                    return default(TResponse);
            }

            return responseDto;
        }

        private async Task<TResponse> ExecAsync<TResponse>(object request)
        {
            foreach (var filter in HostContext.AppHost.GatewayRequestFiltersArray)
            {
                filter(this.Request, request);
                if (this.Request.Response.IsClosed)
                    return default(TResponse);
            }
            foreach (var filter in HostContext.AppHost.GatewayRequestFiltersAsyncArray)
            {
                await filter(this.Request, request);
                if (this.Request.Response.IsClosed)
                    return default(TResponse);
            }

            await ExecValidators(request);

            var response = await HostContext.ServiceController.GatewayExecuteAsync(request, this.Request, applyFilters: false);

            var responseDto = ConvertToResponse<TResponse>(response);

            foreach (var filter in HostContext.AppHost.GatewayResponseFiltersArray)
            {
                filter(this.Request, responseDto);
                if (this.Request.Response.IsClosed)
                    return default(TResponse);
            }
            foreach (var filter in HostContext.AppHost.GatewayResponseFiltersAsyncArray)
            {
                await filter(this.Request, responseDto);
                if (this.Request.Response.IsClosed)
                    return default(TResponse);
            }

            return responseDto;
        }

        private async Task ExecValidators(object request)
        {
            var feature = HostContext.GetPlugin<ValidationFeature>();
            if (feature != null)
            {
                var validator = ValidatorCache.GetValidator(this.Request, request.GetType());
                if (validator != null)
                {
                    var ruleSet = (string) (this.Request.GetItem(Keywords.InvokeVerb) ?? this.Request.Verb);
                    var validationContext = new ValidationContext(request, null, new MultiRuleSetValidatorSelector(ruleSet))
                    {
                        Request = this.Request
                    };
                    
                    ValidationResult result;
                    if (!validator.HasAsyncValidators())
                    {
                        result = validator.Validate(validationContext);
                    }
                    else
                    {
                        result = await validator.ValidateAsync(validationContext);
                    }
                    
                    if (!result.IsValid)
                        throw result.ToWebServiceException(request, feature);
                }
            }
        }

        private TResponse ConvertToResponse<TResponse>(object response)
        {
            if (response is HttpError error)
                throw error.ToWebServiceException();

            var responseDto = response.GetResponseDto();

            return (TResponse) responseDto;
        }

        public TResponse Send<TResponse>(object requestDto)
        {
            var holdDto = this.Request.Dto;
            var holdOp = this.Request.OperationName;
            var holdAttrs = this.Request.RequestAttributes;
            var holdVerb = SetVerb(requestDto);

            this.Request.RequestAttributes |= RequestAttributes.InProcess;
            try
            {
                return ExecSync<TResponse>(requestDto);
            }
            finally
            {
                this.Request.Dto = holdDto;
                this.Request.OperationName = holdOp;
                this.Request.RequestAttributes = holdAttrs;
                ResetVerb(holdVerb);
            }
        }

        public async Task<TResponse> SendAsync<TResponse>(object requestDto, CancellationToken token = new CancellationToken())
        {
            var holdDto = this.Request.Dto;
            var holdOp = this.Request.OperationName;
            var holdAttrs = this.Request.RequestAttributes;
            var holdVerb = SetVerb(requestDto);

            this.Request.RequestAttributes |= RequestAttributes.InProcess;

            try
            {
                var response = await ExecAsync<TResponse>(requestDto);
                return response;
            }
            finally
            {
                this.Request.Dto = holdDto;
                this.Request.OperationName = holdOp;
                this.Request.RequestAttributes = holdAttrs;
                ResetVerb(holdVerb);
            }
        }

        private static object[] CreateTypedArray(IEnumerable<object> requestDtos)
        {
            var requestsArray = requestDtos.ToArray();
            var elType = requestDtos.GetType().GetCollectionType();
            var toArray = (object[])Array.CreateInstance(elType, requestsArray.Length);
            for (var i = 0; i < requestsArray.Length; i++)
            {
                toArray[i] = requestsArray[i];
            }
            return toArray;
        }

        public List<TResponse> SendAll<TResponse>(IEnumerable<object> requestDtos)
        {
            var holdDto = this.Request.Dto;
            var holdAttrs = this.Request.RequestAttributes;
            var holdVerb = this.Request.GetItem(Keywords.InvokeVerb) as string;

            var typedArray = CreateTypedArray(requestDtos);
            this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Post);
            this.Request.RequestAttributes |= RequestAttributes.InProcess;

            try
            {
                return ExecSync<TResponse[]>(typedArray).ToList();
            }
            finally
            {
                this.Request.Dto = holdDto;
                this.Request.RequestAttributes = holdAttrs;
                ResetVerb(holdVerb);
            }
        }

        public Task<List<TResponse>> SendAllAsync<TResponse>(IEnumerable<object> requestDtos, CancellationToken token = new CancellationToken())
        {
            var holdDto = this.Request.Dto;
            var holdAttrs = this.Request.RequestAttributes;
            var holdVerb = this.Request.GetItem(Keywords.InvokeVerb) as string;

            var typedArray = CreateTypedArray(requestDtos);
            this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Post);
            this.Request.RequestAttributes |= RequestAttributes.InProcess;

            var responseTask = ExecAsync<TResponse[]>(typedArray);
            return HostContext.Async.ContinueWith(this.Request, responseTask, task => 
                {
                    this.Request.Dto = holdDto;
                    this.Request.RequestAttributes = holdAttrs;
                    ResetVerb(holdVerb);
                    return task.Result.ToList();
                }, token);
        }

        public void Publish(object requestDto)
        {
            var holdDto = this.Request.Dto;
            var holdOp = this.Request.OperationName;
            var holdAttrs = this.Request.RequestAttributes;
            var holdVerb = SetVerb(requestDto);

            this.Request.RequestAttributes &= ~RequestAttributes.Reply;
            this.Request.RequestAttributes |= RequestAttributes.OneWay;
            this.Request.RequestAttributes |= RequestAttributes.InProcess;

            try
            {
                var response = HostContext.ServiceController.Execute(requestDto, this.Request);
            }
            finally
            {
                this.Request.Dto = holdDto;
                this.Request.OperationName = holdOp;
                this.Request.RequestAttributes = holdAttrs;
                ResetVerb(holdVerb);
            }
        }

        public async Task PublishAsync(object requestDto, CancellationToken token = new CancellationToken())
        {
            var holdDto = this.Request.Dto;
            var holdOp = this.Request.OperationName;
            var holdAttrs = this.Request.RequestAttributes;
            var holdVerb = SetVerb(requestDto);
            
            this.Request.RequestAttributes &= ~RequestAttributes.Reply;
            this.Request.RequestAttributes |= RequestAttributes.OneWay;
            this.Request.RequestAttributes |= RequestAttributes.InProcess;

            await HostContext.ServiceController.GatewayExecuteAsync(requestDto, this.Request, applyFilters: false);

            this.Request.Dto = holdDto;
            this.Request.OperationName = holdOp;
            this.Request.RequestAttributes = holdAttrs;
            ResetVerb(holdVerb);
        }

        public void PublishAll(IEnumerable<object> requestDtos)
        {
            var holdDto = this.Request.Dto;
            var holdAttrs = this.Request.RequestAttributes;
            var holdVerb = this.Request.GetItem(Keywords.InvokeVerb) as string;

            var typedArray = CreateTypedArray(requestDtos);
            this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Post);
            this.Request.RequestAttributes &= ~RequestAttributes.Reply;
            this.Request.RequestAttributes |= RequestAttributes.OneWay;
            this.Request.RequestAttributes |= RequestAttributes.InProcess;

            try
            {
                var response = HostContext.ServiceController.Execute(typedArray, this.Request);
            }
            finally
            {
                this.Request.Dto = holdDto;
                this.Request.RequestAttributes = holdAttrs;
                ResetVerb(holdVerb);
            }
        }

        public async Task PublishAllAsync(IEnumerable<object> requestDtos, CancellationToken token = new CancellationToken())
        {
            var holdDto = this.Request.Dto;
            var holdAttrs = this.Request.RequestAttributes;
            var holdVerb = this.Request.GetItem(Keywords.InvokeVerb) as string;

            var typedArray = CreateTypedArray(requestDtos);
            this.Request.SetItem(Keywords.InvokeVerb, HttpMethods.Post);
            this.Request.RequestAttributes &= ~RequestAttributes.Reply;
            this.Request.RequestAttributes |= RequestAttributes.OneWay;

            await HostContext.ServiceController.GatewayExecuteAsync(typedArray, this.Request, applyFilters: false);

            this.Request.Dto = holdDto;
            this.Request.RequestAttributes = holdAttrs;
            ResetVerb(holdVerb);
        }
    }
}