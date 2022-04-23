// ***********************************************************************
// <copyright file="SqlExpressionVisitor.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace ServiceStack.OrmLite;

//http://blogs.msdn.com/b/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
/// <summary>
/// Class SqlExpressionVisitor.
/// </summary>
public abstract class SqlExpressionVisitor
{
    /// <summary>
    /// Visits the specified exp.
    /// </summary>
    /// <param name="exp">The exp.</param>
    /// <returns>Expression.</returns>
    /// <exception cref="System.Exception">Unhandled expression type: '{exp.NodeType}'</exception>
    protected virtual Expression Visit(Expression exp)
    {
        if (exp == null)
            return exp;
        switch (exp.NodeType)
        {
            case ExpressionType.Negate:
            case ExpressionType.NegateChecked:
            case ExpressionType.Not:
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
            case ExpressionType.ArrayLength:
            case ExpressionType.Quote:
            case ExpressionType.TypeAs:
                return this.VisitUnary((UnaryExpression)exp);
            case ExpressionType.Add:
            case ExpressionType.AddChecked:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.And:
            case ExpressionType.AndAlso:
            case ExpressionType.Or:
            case ExpressionType.OrElse:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.Equal:
            case ExpressionType.NotEqual:
            case ExpressionType.Coalesce:
            case ExpressionType.ArrayIndex:
            case ExpressionType.RightShift:
            case ExpressionType.LeftShift:
            case ExpressionType.ExclusiveOr:
                return this.VisitBinary((BinaryExpression)exp);
            case ExpressionType.TypeIs:
                return this.VisitTypeIs((TypeBinaryExpression)exp);
            case ExpressionType.Conditional:
                return this.VisitConditional((ConditionalExpression)exp);
            case ExpressionType.Constant:
                return this.VisitConstant((ConstantExpression)exp);
            case ExpressionType.Parameter:
                return this.VisitParameter((ParameterExpression)exp);
            case ExpressionType.MemberAccess:
                return this.VisitMemberAccess((MemberExpression)exp);
            case ExpressionType.Call:
                return this.VisitMethodCall((MethodCallExpression)exp);
            case ExpressionType.Lambda:
                return this.VisitLambda((LambdaExpression)exp);
            case ExpressionType.New:
                return this.VisitNew((NewExpression)exp);
            case ExpressionType.NewArrayInit:
            case ExpressionType.NewArrayBounds:
                return this.VisitNewArray((NewArrayExpression)exp);
            case ExpressionType.Invoke:
                return this.VisitInvocation((InvocationExpression)exp);
            case ExpressionType.MemberInit:
                return this.VisitMemberInit((MemberInitExpression)exp);
            case ExpressionType.ListInit:
                return this.VisitListInit((ListInitExpression)exp);
            default:
                throw new Exception($"Unhandled expression type: '{exp.NodeType}'");
        }
    }

    /// <summary>
    /// Visits the binding.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <returns>MemberBinding.</returns>
    /// <exception cref="System.Exception">Unhandled binding type '{binding.BindingType}'</exception>
    protected virtual MemberBinding VisitBinding(MemberBinding binding)
    {
        switch (binding.BindingType)
        {
            case MemberBindingType.Assignment:
                return this.VisitMemberAssignment((MemberAssignment)binding);
            case MemberBindingType.MemberBinding:
                return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
            case MemberBindingType.ListBinding:
                return this.VisitMemberListBinding((MemberListBinding)binding);
            default:
                throw new Exception($"Unhandled binding type '{binding.BindingType}'");
        }
    }

    /// <summary>
    /// Visits the element initializer.
    /// </summary>
    /// <param name="initializer">The initializer.</param>
    /// <returns>ElementInit.</returns>
    protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
    {
        ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
        if (arguments != initializer.Arguments)
        {
            return Expression.ElementInit(initializer.AddMethod, arguments);
        }
        return initializer;
    }

    /// <summary>
    /// Visits the unary.
    /// </summary>
    /// <param name="u">The u.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitUnary(UnaryExpression u)
    {
        Expression operand = this.Visit(u.Operand);
        if (operand != u.Operand)
        {
            return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
        }
        return u;
    }

    /// <summary>
    /// Visits the binary.
    /// </summary>
    /// <param name="b">The b.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitBinary(BinaryExpression b)
    {
        Expression left = this.Visit(b.Left);
        Expression right = this.Visit(b.Right);
        Expression conversion = this.Visit(b.Conversion);
        if (left != b.Left || right != b.Right || conversion != b.Conversion)
        {
            if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                return Expression.Coalesce(left, right, conversion as LambdaExpression);
            else
                return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
        }
        return b;
    }

    /// <summary>
    /// Visits the type is.
    /// </summary>
    /// <param name="b">The b.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
    {
        Expression expr = this.Visit(b.Expression);
        if (expr != b.Expression)
        {
            return Expression.TypeIs(expr, b.TypeOperand);
        }
        return b;
    }

    /// <summary>
    /// Visits the constant.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitConstant(ConstantExpression c)
    {
        return c;
    }

    /// <summary>
    /// Visits the conditional.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitConditional(ConditionalExpression c)
    {
        Expression test = this.Visit(c.Test);
        Expression ifTrue = this.Visit(c.IfTrue);
        Expression ifFalse = this.Visit(c.IfFalse);
        if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
        {
            return Expression.Condition(test, ifTrue, ifFalse);
        }
        return c;
    }

    /// <summary>
    /// Visits the parameter.
    /// </summary>
    /// <param name="p">The p.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitParameter(ParameterExpression p)
    {
        return p;
    }

    /// <summary>
    /// Visits the member access.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitMemberAccess(MemberExpression m)
    {
        Expression exp = this.Visit(m.Expression);
        if (exp != m.Expression)
        {
            return Expression.MakeMemberAccess(exp, m.Member);
        }
        return m;
    }

    /// <summary>
    /// Visits the method call.
    /// </summary>
    /// <param name="m">The m.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitMethodCall(MethodCallExpression m)
    {
        Expression obj = this.Visit(m.Object);
        IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
        if (obj != m.Object || args != m.Arguments)
        {
            return Expression.Call(obj, m.Method, args);
        }
        return m;
    }

    /// <summary>
    /// Visits the expression list.
    /// </summary>
    /// <param name="original">The original.</param>
    /// <returns>ReadOnlyCollection&lt;Expression&gt;.</returns>
    protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
    {
        List<Expression> list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            Expression p = this.Visit(original[i]);
            if (list != null)
            {
                list.Add(p);
            }
            else if (p != original[i])
            {
                list = new List<Expression>(n);
                for (int j = 0; j < i; j++)
                {
                    list.Add(original[j]);
                }
                list.Add(p);
            }
        }
        if (list != null)
        {
            return list.AsReadOnly();
        }
        return original;
    }

    /// <summary>
    /// Visits the member assignment.
    /// </summary>
    /// <param name="assignment">The assignment.</param>
    /// <returns>MemberAssignment.</returns>
    protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
    {
        Expression e = this.Visit(assignment.Expression);
        if (e != assignment.Expression)
        {
            return Expression.Bind(assignment.Member, e);
        }
        return assignment;
    }

    /// <summary>
    /// Visits the member member binding.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <returns>MemberMemberBinding.</returns>
    protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
    {
        IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
        if (bindings != binding.Bindings)
        {
            return Expression.MemberBind(binding.Member, bindings);
        }
        return binding;
    }

    /// <summary>
    /// Visits the member list binding.
    /// </summary>
    /// <param name="binding">The binding.</param>
    /// <returns>MemberListBinding.</returns>
    protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
    {
        IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
        if (initializers != binding.Initializers)
        {
            return Expression.ListBind(binding.Member, initializers);
        }
        return binding;
    }

    /// <summary>
    /// Visits the binding list.
    /// </summary>
    /// <param name="original">The original.</param>
    /// <returns>IEnumerable&lt;MemberBinding&gt;.</returns>
    protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
    {
        List<MemberBinding> list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            MemberBinding b = this.VisitBinding(original[i]);
            if (list != null)
            {
                list.Add(b);
            }
            else if (b != original[i])
            {
                list = new List<MemberBinding>(n);
                for (int j = 0; j < i; j++)
                {
                    list.Add(original[j]);
                }
                list.Add(b);
            }
        }
        if (list != null)
            return list;
        return original;
    }

    /// <summary>
    /// Visits the element initializer list.
    /// </summary>
    /// <param name="original">The original.</param>
    /// <returns>IEnumerable&lt;ElementInit&gt;.</returns>
    protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
    {
        List<ElementInit> list = null;
        for (int i = 0, n = original.Count; i < n; i++)
        {
            ElementInit init = this.VisitElementInitializer(original[i]);
            if (list != null)
            {
                list.Add(init);
            }
            else if (init != original[i])
            {
                list = new List<ElementInit>(n);
                for (int j = 0; j < i; j++)
                {
                    list.Add(original[j]);
                }
                list.Add(init);
            }
        }
        if (list != null)
            return list;
        return original;
    }

    /// <summary>
    /// Visits the lambda.
    /// </summary>
    /// <param name="lambda">The lambda.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitLambda(LambdaExpression lambda)
    {
        Expression body = this.Visit(lambda.Body);
        if (body != lambda.Body)
        {
            return Expression.Lambda(lambda.Type, body, lambda.Parameters);
        }
        return lambda;
    }

    /// <summary>
    /// Visits the new.
    /// </summary>
    /// <param name="nex">The nex.</param>
    /// <returns>NewExpression.</returns>
    protected virtual NewExpression VisitNew(NewExpression nex)
    {
        IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
        if (args != nex.Arguments)
        {
            if (nex.Members != null)
                return Expression.New(nex.Constructor, args, nex.Members);
            else
                return Expression.New(nex.Constructor, args);
        }
        return nex;
    }

    /// <summary>
    /// Visits the member initialize.
    /// </summary>
    /// <param name="init">The initialize.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitMemberInit(MemberInitExpression init)
    {
        NewExpression n = this.VisitNew(init.NewExpression);
        IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
        if (n != init.NewExpression || bindings != init.Bindings)
        {
            return Expression.MemberInit(n, bindings);
        }
        return init;
    }

    /// <summary>
    /// Visits the list initialize.
    /// </summary>
    /// <param name="init">The initialize.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitListInit(ListInitExpression init)
    {
        NewExpression n = this.VisitNew(init.NewExpression);
        IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
        if (n != init.NewExpression || initializers != init.Initializers)
        {
            return Expression.ListInit(n, initializers);
        }
        return init;
    }

    /// <summary>
    /// Visits the new array.
    /// </summary>
    /// <param name="na">The na.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitNewArray(NewArrayExpression na)
    {
        IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
        if (exprs != na.Expressions)
        {
            if (na.NodeType == ExpressionType.NewArrayInit)
            {
                return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
            }
            else
            {
                return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
            }
        }
        return na;
    }

    /// <summary>
    /// Visits the invocation.
    /// </summary>
    /// <param name="iv">The iv.</param>
    /// <returns>Expression.</returns>
    protected virtual Expression VisitInvocation(InvocationExpression iv)
    {
        IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
        Expression expr = this.Visit(iv.Expression);
        if (args != iv.Arguments || expr != iv.Expression)
        {
            return Expression.Invoke(expr, args);
        }
        return iv;
    }
}