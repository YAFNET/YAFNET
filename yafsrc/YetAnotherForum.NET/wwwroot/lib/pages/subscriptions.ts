const _global = (window /* browser */ || global /* node */) as any;

_global.Subscriptions = class Subscriptions {
    TextAppInstalled: string;
    TextSubscribed: string;
    TextBrowserNotSupported: string;
    TextIosHandling: string;
    TextStopped: string;
    TextNotificationsBlocked: string;
    TextNotificationsBlockedMobile: string;
    TextRequestingPermission: string;
    TextPermissionGranted: string;
    TextPermissionDenied: string;
    TextPermissionDeniedMobile: string;
    TextPermissionDismissed: string;

    constructor(
	    textAppInstalled: string,
        textSubscribed: string,
        textBrowserNotSupported: string,
        textIosHandling: string,
        textStopped: string,
        textNotificationsBlocked: string,
        textNotificationsBlockedMobile: string,
        textRequestingPermission: string,
        textPermissionGranted: string,
        textPermissionDenied: string,
        textPermissionDeniedMobile: string,
        textPermissionDismissed: string
    ) {
        this.TextAppInstalled = textAppInstalled;
        this.TextSubscribed = textSubscribed;
        this.TextBrowserNotSupported = textBrowserNotSupported;
        this.TextIosHandling = textIosHandling;
        this.TextStopped = textStopped;
        this.TextNotificationsBlocked = textNotificationsBlocked;
        this.TextNotificationsBlockedMobile = textNotificationsBlockedMobile;
        this.TextRequestingPermission = textRequestingPermission;
        this.TextPermissionGranted = textPermissionGranted;
        this.TextPermissionDenied = textPermissionDenied;
        this.TextPermissionDeniedMobile = textPermissionDeniedMobile;
        this.TextPermissionDismissed = textPermissionDismissed;

        var reg: ServiceWorkerRegistration;


        // Register Service Worker
        if ('serviceWorker' in navigator) {
            navigator.serviceWorker.register('/serviceworker')
                .then(registration => {
                    reg = registration;

                    if (Notification.permission === 'granted') {
                        getSubscription(reg, this.TextSubscribed);
                    }
                })
                .catch(error => {
                    console.log('Service Worker registration failed:', error);
                });
        }

        let notificationInterval: any = null;

        const statusDiv = document.getElementById('status')!;
        const enableBtn = document.getElementById('enableBtn')!;
        const enableText = document.getElementById('enableText')!;
        const stopBtn = document.getElementById('stopBtn')!;
        const installBtn = document.getElementById('installBtn')!;
        const mobileInstructions = document.getElementById('mobileInstructions')!;
        const applicationServerKey = (document.getElementById('ApplicationServerKey') as HTMLInputElement).value;

       


        // Detect mobile
        const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
        const isIOS = /iPhone|iPad|iPod/i.test(navigator.userAgent);

        const navigatr: any = window.navigator;

        const isStandalone = window.matchMedia('(display-mode: standalone)').matches || navigatr.standalone;

        if (isMobile) {
            mobileInstructions.classList.remove('d-none');
        }

        // PWA Install prompt
        let deferredPrompt: any;


        installBtn.addEventListener('click', async () => {
            if (deferredPrompt) {
                deferredPrompt.prompt();
                const { outcome } = await deferredPrompt.userChoice;
                if (outcome === 'accepted') {
                    showStatus(this.TextAppInstalled, 'success');
                }
                deferredPrompt = null;
                installBtn.style.display = 'none';
            }
        });

        function showStatus(msg: string, type: string) {
            statusDiv.textContent = msg;
            statusDiv.className = `alert alert-${type} mt-3`;
        }

        function getSubscription(reg: ServiceWorkerRegistration, textSubscribed: string) {
            reg.pushManager.getSubscription().then(sub => {
                if (sub === null) {
                    reg.pushManager.subscribe({
                        userVisibleOnly: true,
                        applicationServerKey
                    }).then(subs => {
                        pushSubscription(subs, textSubscribed);
                    }).catch(error => {
                        console.error('Unable to subscribe to push', error);
                    });
                } else {
                    pushSubscription(sub, textSubscribed);
                }
            });

            enableBtn.style.display = 'none';
            enableText.style.display = 'none';
            stopBtn.style.display = 'inline-block';
        }

        function pushSubscription(sub: PushSubscription, textSubscribed: string) {
            const deviceSubscription: { device: string; userID: number; boardID: number; endPoint: string, p256dh: string, auth: string } = {
                device: '',
                userID: 0,
                boardID: 0,
                endPoint: sub.endpoint,
                p256dh: arrayBufferToBase64(sub.getKey('p256dh')!),
                auth: arrayBufferToBase64(sub.getKey('auth')!)
            };

            const ajaxUrl = '/api/Subscription/SubscribeDevice';

            fetch(ajaxUrl,
                {
                    method: 'POST',
                    body: JSON.stringify(deviceSubscription),
                    headers: {
                        Accept: 'application/json',
                        "Content-Type": 'application/json;charset=utf-8',
                        "RequestVerificationToken": (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
                    }
                }).then(data => {
                    showStatus(textSubscribed, 'success');
                }).catch(error => {
                    console.log(error);
                });
        }

        function arrayBufferToBase64(buffer: ArrayBuffer): string {
            let binary = '';
            const bytes = new Uint8Array(buffer);
            const len = bytes.byteLength;
            for (let i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
        }

        function stopNotifications(textStopped: string) {
            if (notificationInterval) {
                clearInterval(notificationInterval);
                notificationInterval = null;
            }
            enableBtn.style.display = 'inline-block';
            stopBtn.style.display = 'none';

            // Unsubscribe from push notifications on server and client
            reg.pushManager.getSubscription().then(sub => {
                if (sub != null) {
                    const deviceSubscription: { device: string; userID: number; boardID: number; endPoint: string, p256dh: string, auth: string } = {
                        device: '',
                        userID: 0,
                        boardID: 0,
                        endPoint: sub.endpoint,
                        p256dh: arrayBufferToBase64(sub.getKey('p256dh')!),
                        auth: arrayBufferToBase64(sub.getKey('auth')!)
                    };

                    const ajaxUrl = '/api/Subscription/UnSubscribeDevice';

                    fetch(ajaxUrl,
                        {
                            method: 'POST',
                            body: JSON.stringify(deviceSubscription),
                            headers: {
                                Accept: 'application/json',
                                "Content-Type": 'application/json;charset=utf-8',
                                "RequestVerificationToken": (document.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement).value
                            }
                        }).then(data => {
                            showStatus(textStopped, 'info');
                        }).catch(error => {
                            console.log(error);
                        });
                }
            });
        }

        enableBtn.addEventListener('click', async () => {
            if (!('Notification' in window)) {
                showStatus(this.TextBrowserNotSupported, 'danger');
                return;
            }

            // Special handling for iOS
            if (isIOS && !isStandalone) {
                showStatus(this.TextIosHandling, 'danger');
                return;
            }

            const currentPermission = Notification.permission;

            // If already denied, show instructions
            if (currentPermission === 'denied') {
                if (isMobile) {
                    showStatus(this.TextNotificationsBlocked, 'danger');
                } else {
                    showStatus(this.TextNotificationsBlockedMobile, 'danger');
                }
                return;
            }

            // If already granted, start immediately
            if (currentPermission === 'granted') {
                getSubscription(reg, this.TextSubscribed);
                return;
            }

            // Request permission (only works if permission is 'default')
            try {
                showStatus(this.TextRequestingPermission, 'info');

                // Add a small delay for mobile
                if (isMobile) {
                    await new Promise(resolve => setTimeout(resolve, 500));
                }

                const permission = await Notification.requestPermission();

                if (permission === 'granted') {
                    showStatus(this.TextPermissionGranted, 'success');
                    // setTimeout(startNotifications, 500);
                    getSubscription(reg, this.TextSubscribed);
                } else if (permission === 'denied') {
                    if (isMobile) {
                        showStatus(this.TextPermissionDeniedMobile, 'danger');
                    } else {
                        showStatus(this.TextPermissionDenied, 'danger');
                    }
                } else {
                    showStatus(this.TextPermissionDismissed, 'danger');
                }
            } catch (error: any) {
                showStatus(`❌ ${error.message}`, 'danger');
            }
        });

        stopBtn.addEventListener('click',
	        () => {
		        stopNotifications(this.TextStopped);
	        });
    }
}