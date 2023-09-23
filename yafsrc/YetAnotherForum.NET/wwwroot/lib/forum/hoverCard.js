function userCardContent(pop, delay) {
    fetch(pop.dataset.hovercard,
            {
                method: "GET",
                headers: {
                    "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value,
                    "Accept": "application/json",
                    "Content-Type": "application/json;charset=utf-8"
                }
            }).then(res => res.json())
        .then(profileData => {

            var content =
                (profileData.avatar ? `<img src="${profileData.avatar}" class="rounded mx-auto d-block" style="width:75px" alt="" />` : "") +
                    '<ul class="list-unstyled m-0">' +
                    (profileData.location ? `<li class="px-2 py-1"><i class="fas fa-home me-1"></i>${profileData.location}</li>` : "") +
                    (profileData.rank ? `<li class="px-2 py-1"><i class="fas fa-graduation-cap me-1"></i>${profileData.rank}</li>` : "") +
                    (profileData.interests ? `<li class="px-2 py-1"><i class="fas fa-running me-1"></i>${profileData.interests}</li>` : "") +
                    (profileData.joined ? `<li class="px-2 py-1"><i class="fas fa-user-check me-1"></i>${profileData.joined}</li>` : "") +
                    (profileData.homePage ? `<li class="px-2 py-1"><i class="fas fa-globe me-1"></i><a href="${profileData.homePage}" target="_blank">${profileData.homePage}</a></li>` : "") +
                    '<li class="px-2 py-1"><i class="far fa-comment me-1"></i>' + profileData.posts + "</li>" +
                    "</ul>";


            const popover = new bootstrap.Popover(pop,
                {
                    delay: { "show": delay, "hide": 100 },
                    trigger: "hover",
                    html: true,
                    content: content,
                    template: '<div class="popover" role="tooltip"><div class="popover-arrow"></div><div class="popover-body p-2"></div></div>'
                });


        }).catch(function (error) {
            console.log(error);
        });
}