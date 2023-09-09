function userCardContent(pop, delay) {
    fetch(pop.dataset.hovercard,
            {
                method: "GET",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json;charset=utf-8"
                }
            }).then(res => res.json())
        .then(profileData => {

            var content =
                (profileData.Avatar ? `<img src="${profileData.Avatar}" class="rounded mx-auto d-block" style="width:75px" alt="" />` : "") +
                    '<ul class="list-unstyled m-0">' +
                    (profileData.Location ? `<li class="px-2 py-1"><i class="fas fa-home me-1"></i>${profileData.Location}</li>` : "") +
                    (profileData.Rank ? `<li class="px-2 py-1"><i class="fas fa-graduation-cap me-1"></i>${profileData.Rank}</li>` : "") +
                    (profileData.Interests ? `<li class="px-2 py-1"><i class="fas fa-running me-1"></i>${profileData.Interests}</li>` : "") +
                    (profileData.Joined ? `<li class="px-2 py-1"><i class="fas fa-user-check me-1"></i>${profileData.Joined}</li>` : "") +
                    (profileData.HomePage ? `<li class="px-2 py-1"><i class="fas fa-globe me-1"></i><a href="${profileData.HomePage}" target="_blank">${profileData.HomePage}</a></li>` : "") +
                    '<li class="px-2 py-1"><i class="far fa-comment me-1"></i>' + profileData.Posts + "</li>" +
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