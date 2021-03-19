<script lang="ts">
  import { fade } from "svelte/transition";

  let overlay: boolean = false;

  export let openOverlayCallback: () => void;
  export const openOverlay = () => {
    overlay = true;
    document.body.style.top = `-${document.body.parentElement.scrollTop}px`;
    document.body.style.height = `height: calc(100% + ${document.body.parentElement.scrollTop}px)`;
    document.body.classList.add("noscroll");

    if (openOverlayCallback) {
      openOverlayCallback();
    }
  };

  export const closeOverlay = () => {
    overlay = false;
  };
</script>

{#key overlay}
  <div
    class="hidden"
    class:overlay
    transition:fade
    on:outroend={() => {
      if (!overlay) {
        document.body.classList.remove("noscroll");
        document.body.parentElement.scrollTop = parseInt(
          document.body.style.top.slice(1, -2)
        );
        document.body.style.top = "";
      }
    }}
    on:click={() => {
      overlay = false;
    }}>
    <div
      class="modal group column"
      on:click={(event) => event.stopPropagation()}>
      <slot />
    </div>
    <div class="blocker" on:click={closeOverlay} />
  </div>
{/key}

<style>
  :global(.noscroll) {
    position: fixed;
    width: 100%;
    height: 100%;
    overflow-y: scroll;
  }

  .group {
    display: flex;
  }

  .column {
    flex-direction: column;
  }

  .hidden:not(.overlay) {
    display: none;
  }

  .overlay {
    position: fixed;
    display: flex;
    z-index: 10;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    flex-direction: column;
    justify-content: center;
  }

  .modal {
    margin: 0 auto;
    width: calc((1vh + 1vw) * 20);
    border-radius: calc((1vh + 1vw) * 0.5);
    background: linear-gradient(
      135deg,
      hsl(22deg, 9%, 98%),
      hsl(19deg 50% 70%)
    );
    color: #a75b37;
    z-index: 1;
    padding: calc((1vh + 1vw) * 0.5);
  }

  .blocker {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: #2f190e61;
  }
</style>
