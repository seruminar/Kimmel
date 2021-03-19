<script lang="ts">
  import { isArray, isPlainObject, sortBy } from "lodash";
  import { property } from "../actions/property";
  import ChevronDown from "./chevronDown.svelte";
  import { fade, fly } from "svelte/transition";

  export let label: string = "";
  export let object: any;
  export let depth: number = 1;

  export let sortObjectKeys: (key: string) => any;
  export let getObjectLabel: (key: any) => string = () => "";
  export let backgroundHover: string;
  export let checkedColor: string;
  export let color: string;

  export let checked: boolean;
</script>

{#if isArray(object)}
  <div
    class="group"
    transition:fly={{ y: -10 }}
    use:property={["depth", `calc((${depth - 1}vw + ${depth - 1}vh) / 2)`]}
    use:property={["backgroundHover", backgroundHover]}
    use:property={["checkedColor", checkedColor]}
    use:property={["color", color]}>
    {#if object.length > 0}
      <label transition:fade>
        <input type="checkbox" bind:checked />
        <div class="checkbox">
          <ChevronDown />
        </div>
      </label>
    {/if}
    <div class="depth" />
    {#if label !== ""}
      <b>{label}</b>
    {/if}
    <span>
      {object.length}
    </span>
    <div class="item" />
  </div>
  {#if checked}
    {#each object as pair, index}
      <svelte:self
        {...$$props}
        checked={undefined}
        label={index + 1}
        object={pair}
        depth={depth + 1} />
    {/each}
  {/if}
{:else if isPlainObject(object)}
  <div
    class="group"
    transition:fly={{ y: -10 }}
    use:property={["depth", `calc((${depth - 1}vw + ${depth - 1}vh) / 2)`]}
    use:property={["backgroundHover", backgroundHover]}
    use:property={["checkedColor", checkedColor]}
    use:property={["color", color]}>
    <label transition:fade>
      <input type="checkbox" bind:checked />
      <div class="checkbox">
        <ChevronDown />
      </div>
    </label>
    <div class="depth" />
    {#if label !== ""}
      <b>{label}</b>
    {/if}
    <span>
      {getObjectLabel(object)}
    </span>
    <div class="item" />
  </div>
  {#if checked}
    {#each sortBy(Object.keys(object), sortObjectKeys) as key}
      <svelte:self
        {...$$props}
        checked={undefined}
        label={key}
        object={object[key]}
        depth={depth + 1} />
    {/each}
  {/if}
{:else}
  <div
    class="group"
    transition:fly={{ y: -10 }}
    use:property={["depth", `calc((${depth - 1}vw + ${depth - 1}vh) / 2)`]}
    use:property={["backgroundHover", backgroundHover]}
    use:property={["checkedColor", checkedColor]}
    use:property={["color", color]}>
    <div class="depth" />
    {#if label !== ""}
      <b>{label}</b>
    {/if}
    <span>{object}</span>
    <div class="item" />
  </div>
{/if}

<style>
  .group {
    display: flex;
    min-height: calc((1vw + 1vh) * 1);
    padding: calc((1vw + 1vh) * 0.1) 0;
  }

  .group:hover {
    background: var(--backgroundHover);
  }

  .item {
    flex: 1;
  }

  label {
    margin: 0 calc((1vw + 1vh) * -0.75) 0 0;
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
    display: inline-block;
    margin: 0 0 0 calc((1vw + 1vh) * 0.75);
    display: table;
  }

  b::after {
    content: ":";
    height: calc((1vw + 1vh) * -0.75);
    margin: 0 calc((1vw + 1vh) * 0.25) 0 calc((1vw + 1vh) * 0.1);
  }
</style>
