
class ServiceWorkerHelper {
    static registerServiceWorker(){
        if ('serviceWorker' in navigator) {
            navigator.serviceWorker
                .register(__globals.SERVICE_WORKER_ROOT)
                .then(function() {
                    console.log('Service worker registered...');
                });
        }
    }
}
export default ServiceWorkerHelper;
