(function () {
  if (!('serviceWorker' in navigator)) {
    return;
  }

  const register = function () {
    navigator.serviceWorker
      .register('/service-worker.js')
      .catch(function (error) {
        console.error('Service worker registration failed:', error);
      });
  };

  if (document.readyState === 'complete') {
    register();
  } else {
    window.addEventListener('load', register);
  }
})();
