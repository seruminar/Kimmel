<script lang="ts">
  import { fade } from "svelte/transition";
</script>

<div class="root" in:fade>
  <div class="loading">
    <span class="letter">k</span>
    <span class="letter">i</span>
    <span class="letter">m</span>
    <span class="letter">m</span>
    <span class="letter">e</span>
    <span class="letter">l</span>
  </div>
</div>

<style>
  .root {
    display: flex;
    height: 100%;
    flex-grow: 1;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background: hsl(19deg 50% 90% / 65%);
    border-radius: calc((1vh + 1vw) * 0.5);
    overflow: hidden;
    cursor: wait;
  }

  .loading {
    color: #a75b37;
  }

  .letter {
    animation: letter 3s ease infinite;
    font-size: calc((1vh + 1vw) * 2);
    font-weight: 1000;
    text-transform: uppercase;
    display: inline-block;
  }

  .letter:nth-child(1) {
    animation-delay: -2.75s;
  }

  .letter:nth-child(2) {
    animation-delay: -2.5s;
  }

  .letter:nth-child(3) {
    animation-delay: -2.25s;
  }

  .letter:nth-child(4) {
    animation-delay: -2s;
  }

  .letter:nth-child(5) {
    animation-delay: -1.75s;
  }

  .letter:nth-child(6) {
    animation-delay: -1.5s;
  }

  @keyframes letter {
    0% {
      opacity: 100%;
      transform: scale(1);
    }
    90% {
      opacity: 0%;
      transform: scale(1.6);
    }
    100% {
      opacity: 100%;
      transform: scale(1);
    }
  }
</style>
