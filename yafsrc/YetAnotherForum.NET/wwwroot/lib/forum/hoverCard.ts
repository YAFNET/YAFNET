import * as bootstrap from 'bootstrap';

const _global = (window /* browser */ || global /* node */) as any;
function userCardContent(pop: HTMLElement, delay: number): void {
    const popover = new bootstrap.Popover(pop, {
        delay: {
            show: delay,
            hide: 100
        },
        trigger: 'hover',
        html: true,
        content: '&nbsp;',
        template: '<div class="popover popover-user" role="tooltip"><div class="popover-arrow"></div><div class="popover-body p-2"></div></div>'
    });

    pop.addEventListener('show.bs.popover', () => {
	    if ((popover as any)._newContent) {
            return;
        }
        fetch(pop.dataset.hovercard as string, {
            method: 'GET',
            headers: {
                RequestVerificationToken: (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value,
                Accept: 'application/json',
                "Content-Type": 'application/json;charset=utf-8'
            }
        }).then(res => res.json()).then((profileData: { avatar?: string, location?: string, rank?: string, interests?: string, joined?: string, homePage?: string, medals?: string, posts?: number }) => {
            const content = (profileData.avatar ? `<img src="${profileData.avatar}" class="rounded mx-auto d-block" style="width:75px" alt="" />` : '')
                + '<ul class="list-unstyled m-0">'
                + (profileData.location ? `<li class="px-2 py-1"><i class="fas fa-home me-1"></i>${profileData.location}</li>` : '')
                + (profileData.rank ? `<li class="px-2 py-1"><i class="fas fa-graduation-cap me-1"></i>${profileData.rank}</li>` : '')
                + (profileData.interests ? `<li class="px-2 py-1"><i class="fas fa-running me-1"></i>${profileData.interests}</li>` : '')
                + (profileData.joined ? `<li class="px-2 py-1"><i class="fas fa-user-check me-1"></i>${profileData.joined}</li>` : '')
                + (profileData.homePage ? `<li class="px-2 py-1"><i class="fas fa-globe me-1"></i><a href="${profileData.homePage}" target="_blank">${profileData.homePage}</a></li>` : '')
                + (profileData.medals ? `<li class="px-2 py-1"><i class="fas fa-medal me-1"></i>${profileData.medals}</li>` : '')
                + `<li class="px-2 py-1"><i class="far fa-comment me-1"></i>${profileData.posts ?? ''}</li></ul>`;

            popover.setContent({ '.popover-body': content });

        }).catch(error => {
            console.error(error);
        });
    });
}

_global.userCardContent = userCardContent;