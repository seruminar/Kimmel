<svelte:head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width,initial-scale=1.0" />
  <meta name="theme-color" content="#333333" />
  <link rel="icon" type="image/png" href="favicon.png" />
</svelte:head>

<slot />

<style>
  @font-face {
    font-family: GT-Walsheim;
    src: url("https://assets-us-01.kc-usercontent.com/561c4b7b-95a6-0021-9ac3-b3ba96ae14b4/72dfa511-92fd-449e-ab3e-da3a6865de8a/GT-Walsheim-Bold.woff2")
        format("woff2"),
      url(https://assets-us-01.kc-usercontent.com/561c4b7b-95a6-0021-9ac3-b3ba96ae14b4/49c3066a-ad9c-4cfb-8ba1-321c58c9a450/GT-Walsheim-Bold.woff)
        format("woff"),
      url(https://assets-us-01.kc-usercontent.com/561c4b7b-95a6-0021-9ac3-b3ba96ae14b4/b904d734-2ae1-411c-b1c6-221bc11565e7/GT-Walsheim-Bold.eot)
        format("embedded-opentype"),
      url(https://assets-us-01.kc-usercontent.com/561c4b7b-95a6-0021-9ac3-b3ba96ae14b4/61249c42-e9f8-4d1e-be0c-2b0734c74c5a/GT-Walsheim-Bold.ttf)
        format("truetype");
    font-weight: 700;
    font-style: normal;
    font-display: swap;
  }
  @font-face {
    font-family: GT-Walsheim;
    src: url("https://assets-us-01.kc-usercontent.com/561c4b7b-95a6-0021-9ac3-b3ba96ae14b4/de4ae192-6c9e-441a-b879-39e50cac7e7c/GT-Walsheim-Regular.woff2")
        format("woff2"),
      url(https://assets-us-01.kc-usercontent.com/561c4b7b-95a6-0021-9ac3-b3ba96ae14b4/d455cc57-046b-4151-849b-88b587c87e86/GT-Walsheim-Regular.woff)
        format("woff"),
      url(https://assets-us-01.kc-usercontent.com/561c4b7b-95a6-0021-9ac3-b3ba96ae14b4/9990d2c6-088d-4b9c-8f6e-e9eaa27196f5/GT-Walsheim-Regular.eot)
        format("embedded-opentype"),
      url(https://assets-us-01.kc-usercontent.com/561c4b7b-95a6-0021-9ac3-b3ba96ae14b4/107f520c-81e5-4390-901b-6fd71fbe10a2/GT-Walsheim-Regular.ttf)
        format("truetype");
    font-weight: 400;
    font-style: normal;
    font-display: swap;
  }

  :global(html) {
    height: 100%;
    display: flex;
    flex-direction: column;
  }

  :global(body) {
    margin: 0;
    font-size: 14px;
    line-height: 1.5;
    color: #333;
    display: flex;
    flex: 1;
    font-family: GT-Walsheim, sans-serif;
  }

  :global(main) {
    position: relative;
    margin: 0 auto;
    box-sizing: border-box;
    display: flex;
    flex-direction: column;
    flex: 1;
  }

  :global(h1),
  :global(h2),
  :global(h3),
  :global(h4),
  :global(h5),
  :global(h6) {
    margin: 0 0 0.5em 0;
    line-height: 1.2;
  }

  :global(a) {
    color: inherit;
  }

  :global(*) {
    box-sizing: border-box;
  }

  :global(section) {
    display: flex;
    flex-direction: column;
    max-width: 1600px;
    margin: 0 auto;
  }

  @media (max-width: 1300px) {
    :global(section) {
      max-width: 900px;
      flex-direction: column !important;
      margin: 0 auto !important;
    }
  }

  @media (min-width: 400px) {
    :global(body) {
      font-size: 16px;
    }
  }
</style>
