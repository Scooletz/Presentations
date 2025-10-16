const CACHE_NAME = 'presentations-offline-cache';
const PRECACHE_URLS = ['/', '/index.html'];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME).then(cache => cache.addAll(PRECACHE_URLS)).catch(() => undefined)
  );
  self.skipWaiting();
});

self.addEventListener('activate', event => {
  event.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', event => {
  const { request } = event;

  if (request.method !== 'GET') {
    return;
  }

  event.respondWith(
    fetch(request)
      .then(response => {
        const shouldCache = response.ok || response.type === 'opaque';

        if (shouldCache) {
          const responseClone = response.clone();
          event.waitUntil(
            caches.open(CACHE_NAME).then(cache => cache.put(request, responseClone)).catch(() => undefined)
          );
        }

        return response;
      })
      .catch(async () => {
        const cachedResponse = await caches.match(request);
        if (cachedResponse) {
          return cachedResponse;
        }

        if (request.mode === 'navigate') {
          const fallbackResponse = await caches.match('/index.html');
          if (fallbackResponse) {
            return fallbackResponse;
          }
        }

        return new Response('Offline', {
          status: 503,
          statusText: 'Offline',
          headers: { 'Content-Type': 'text/plain' }
        });
      })
  );
});
