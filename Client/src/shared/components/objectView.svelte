<script lang="ts" context="module">
  export type ViewMode = "object" | "array" | "primitive";
</script>

<script lang="ts">
  import { isArray, isPlainObject } from "lodash";
  import { property } from "../actions/property";
  import ChevronDown from "./chevronDown.svelte";
  import { fade, fly } from "svelte/transition";

  export let label: string = "";
  export let object: any;
  export let key: any;
  export let depth: number = 1;

  export let getKeys: (object: any, mode: ViewMode) => string[] = (object) =>
    Object.keys(object);
  export let labelCallback: (
    element: HTMLElement,
    object: any,
    key: string,
    mode: ViewMode
  ) => void;
  export let backgroundHover: string;
  export let checkedColor: string;
  export let color: string;

  export let open: boolean;

  let mode: ViewMode;

  $: {
    if (isArray(object)) {
      mode = "array";
    } else if (isPlainObject(object)) {
      mode = "object";
    } else {
      mode = "primitive";
    }
  }

  $: keys = getKeys(object, mode);

  const callback = (
    node: HTMLDivElement,
    _: { object: any; key: string; mode: ViewMode }
  ) => {
    labelCallback(node, object, key, mode);
  };
</script>

<div
  class="group"
  transition:fly={{ y: -10 }}
  use:property={["depth", `calc((1vw + 1vh) * ${depth - 1})`]}
  use:property={["backgroundHover", backgroundHover]}
  use:property={["checkedColor", checkedColor]}
  use:property={["color", color]}>
  {#if mode === "array" && object.length > 0}
    <label transition:fade>
      <input type="checkbox" bind:checked={open} />
      <div class="checkbox">
        <ChevronDown />
      </div>
    </label>
  {:else if mode === "object" && keys.length > 0}
    <label>
      <input type="checkbox" bind:checked={open} />
      <div class="checkbox">
        <ChevronDown />
      </div>
    </label>
  {:else}
    <div class="spacer" />
  {/if}
  <div class="depth" />
  {#if label !== ""}
    <b>{label}</b>
  {/if}
  {#key object}
    {#if object !== undefined}
      <div use:callback={{ object, key, mode }} />
    {/if}
  {/key}
  <div class="item" />
</div>

{#if open && object !== undefined && object !== null}
  {#if mode === "array"}
    {#each keys as key, index}
      <svelte:self
        {...$$props}
        open={undefined}
        label={index + 1}
        object={object[key]}
        {key}
        depth={depth + 1} />
    {/each}
  {:else if mode === "object"}
    {#each keys as key}
      <svelte:self
        {...$$props}
        open={undefined}
        label={key}
        object={object[key]}
        {key}
        depth={depth + 1} />
    {/each}
  {/if}
{/if}

<style>
  .group {
    display: flex;
    min-height: calc((1vw + 1vh) * 1);
    padding: calc((1vw + 1vh) * 0.1) calc((1vw + 1vh) * 0.5)
      calc((1vw + 1vh) * 0.1) 0;
    align-items: center;
    margin-bottom: calc((1vw + 1vh) * 0.1);
    background-color: #fff;
    border-radius: calc((1vw + 1vh) * 0.3);
    box-shadow: 0 0 8px 0 rgb(0 0 0 / 3%);
  }

  .group:hover {
    background: var(--backgroundHover);
  }

  .item {
    flex: 1;
  }

  label {
    margin: 0 calc((1vw + 1vh) * 0.2) 0;
  }

  .spacer {
    margin: 0 calc((1vw + 1vh) * 0.2) 0;
    width: calc((1vw + 1vh) * 0.75);
  }

  input[type="checkbox"] {
    display: none;
  }

  input[type="checkbox"] + .checkbox {
    width: calc((1vw + 1vh) * 0.75);
    height: calc((1vw + 1vh) * 0.75);
  }

  input[type="checkbox"] + .checkbox:hover {
    cursor: pointer;
  }

  input[type="checkbox"] + .checkbox :global(svg) {
    width: 100%;
    height: 100%;
    fill: var(--color);
    transition: transform 0.3s;
    transform: rotate(-90deg);
  }

  input[type="checkbox"]:checked + .checkbox :global(svg) {
    fill: var(--checkedColor);
    transform: rotate(0deg);
  }

  .depth {
    width: var(--depth);
    margin: 0 0 0 calc((1vw + 1vh) * 0.75);
    display: table;
  }

  b::after {
    content: ":";
    height: calc((1vw + 1vh) * -0.75);
    margin: 0 calc((1vw + 1vh) * 0.25) 0 calc((1vw + 1vh) * 0.1);
  }
</style>
