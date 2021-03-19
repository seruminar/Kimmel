<svelte:head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width,initial-scale=1.0" />
  <meta name="theme-color" content="#333333" />
  <link rel="icon" type="image/png" href="favicon.png" />
</svelte:head>

<slot />

<style>
  :global(html) {
    height: 100%;
    display: flex;
    flex-direction: column;
  }

  :global(body) {
    margin: 0;
    font-family: Roboto, -apple-system, BlinkMacSystemFont, Segoe UI, Oxygen,
      Ubuntu, Cantarell, Fira Sans, Droid Sans, Helvetica Neue, sans-serif;
    font-size: 14px;
    line-height: 1.5;
    color: #333;
    display: flex;
    flex: 1;
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
