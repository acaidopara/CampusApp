
self.importScripts('./service-worker-assets.js');

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [
    /\.dll$/, /\.pdb$/, /\.wasm$/, /\.html$/, /\.js$/, /\.json$/,
    /\.css$/, /\.woff$/, /\.woff2$/, /\.ttf$/, /\.otf$/,
    /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/
];
const offlineAssetsExclude = [/^service-worker\.js$/];

const base = "/";
const baseUrl = new URL(base, self.origin);
const manifestUrlList = self.assetsManifest.assets
    .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
    .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
    .map(asset => new URL(asset.url, baseUrl).href);

self.addEventListener('install', event => {
    event.waitUntil((async () => {
        const cache = await caches.open(cacheName);
        const requests = manifestUrlList.map(url => new Request(url, { cache: 'no-cache' }));
        await cache.addAll(requests);
        console.info('Service worker: Installed and cached frontend assets');
    })());
});

self.addEventListener('activate', event => {
    event.waitUntil((async () => {
        const keys = await caches.keys();
        await Promise.all(
            keys.filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
                .map(key => caches.delete(key))
        );
        console.info('Service worker: Activated');
    })());
});

self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') return;

    const url = new URL(event.request.url);

    if (url.pathname.startsWith('/api/')) return;

    event.respondWith((async () => {
        const cache = await caches.open(cacheName);
        const cachedResponse = await cache.match(event.request);
        if (cachedResponse) return cachedResponse;

        try {
            const response = await fetch(event.request);
            if (/\.(png|jpg|jpeg|gif|woff|woff2|css|js)$/.test(url.pathname)) {
                const assetCache = await caches.open('assets-cache');
                assetCache.put(event.request, response.clone());
            }
            return response;
        } catch {
            return cachedResponse || new Response('Offline', { status: 503 });
        }
    })());
});
