<script lang="ts">
  import { without } from "lodash";

  type TValue = any;
  type TOption = { display: string; key: string; filter: string };

  export let id: string = "";
  export let placeholder: string = "";
  export let value: TValue | TValue[];
  export let values: TValue[];
  export let mapOption: (item: TValue) => TOption;
  export let getValue: (rawValue: string) => TValue;
  export let setValueOverride: (
    setValue: (newValue: TValue, add?: boolean) => void,
    newValue: TValue,
    add: boolean
  ) => void;
  export let allowCustom: boolean = false;

  let input: HTMLInputElement;
  let clicked: boolean = false;
  let focused: boolean = false;
  let blurred: boolean = false;
  let rawValue: string = "";

  $: single = !Array.isArray(value);
  $: options = getOptions(values, value);
  $: matchesFilter = rawValue.toLowerCase();

  const getOptions = (values: TValue[], value: TValue | TValue[]) => {
    const optionsMap = values.map((option) => [option, mapOption(option)]) as [
      TValue,
      TOption
    ][];

    if (allowCustom && value !== undefined) {
      if (single) {
        if (!values.some((valuesValue) => valuesValue === value)) {
          optionsMap.push([value as TValue, mapOption(value as TValue)]);
        }
      } else {
        for (const subValue of value as TValue[]) {
          if (!values.includes(subValue)) {
            optionsMap.push([subValue, mapOption(subValue)]);
          }
        }
      }
    }

    return optionsMap;
  };

  const clicking = (data: TValue, add: boolean = true) => {
    setValue(data, add);

    if (single) {
      rawValue = "";
    }

    clicked = true;

    if (blurred && focused) {
      input.focus();
    }

    blurred = false;
  };

  const focusing = () => {
    if (blurred && focused && clicked && single && value !== undefined) {
      input.blur();
    }

    clicked = false;

    if (!blurred && focused) {
      input.blur();
    } else {
      blurred = false;
      focused = true;
    }
  };

  const blurring = () => {
    blurred = true;

    setTimeout(() => {
      if (blurred) {
        if (rawValue !== "") {
          if (getValue) {
            const newValue = getValue(rawValue);

            if (allowCustom) {
              if (newValue) {
                setValue(newValue);
              }
            } else {
              const foundValue = values.find((value) => value === newValue);

              if (foundValue) {
                setValue(foundValue);
              }
            }
          }

          rawValue = "";
        }

        blurred = false;
        focused = false;
      }
    }, 150);
  };

  const setValue = (newValue: TValue, add: boolean = true) => {
    if (setValueOverride) {
      setValueOverride(setValueDirectly, newValue, add);
    } else {
      setValueDirectly(newValue, add);
    }
  };

  const setValueDirectly = (newValue: TValue, add: boolean = true) => {
    if (single) {
      if (add) {
        value = newValue;
      } else {
        value = undefined;
      }
    } else {
      if (add && !value.includes(newValue)) {
        value = [...value, newValue];
      } else {
        value = without(value, newValue);
      }
    }
  };

  const matches = (pair: [TValue, TOption]) => {
    const [, option] = pair;

    let matches = true;

    const optionTokens = option.filter.split("");
    const matchesTokens = matchesFilter.split("");

    for (const token of matchesTokens) {
      if (
        optionTokens.filter((l) => l === token).length <
        matchesTokens.filter((l) => l === token).length
      ) {
        matches = false;
      }
    }

    return matches;
  };
</script>

<div class="root">
  <div class="inputRow">
    <div class="selection" class:focused>
      {#if value}
        {#if single}
          <b class="value">
            <span>
              {options.find((pair) => pair[0] === value)[1].display}
            </span>
            <button on:click={() => clicking(value, false)}
              ><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 352 512"
                ><path
                  d="M242.72 256l100.07-100.07c12.28-12.28 12.28-32.19 0-44.48l-22.24-22.24c-12.28-12.28-32.19-12.28-44.48 0L176 189.28 75.93 89.21c-12.28-12.28-32.19-12.28-44.48 0L9.21 111.45c-12.28 12.28-12.28 32.19 0 44.48L109.28 256 9.21 356.07c-12.28 12.28-12.28 32.19 0 44.48l22.24 22.24c12.28 12.28 32.2 12.28 44.48 0L176 322.72l100.07 100.07c12.28 12.28 32.2 12.28 44.48 0l22.24-22.24c12.28-12.28 12.28-32.19 0-44.48L242.72 256z" /></svg
              ></button>
          </b>
        {:else}
          {#each value as subValue}
            <b class="value">
              <span>
                {options.find((pair) => pair[0] === subValue)[1].display}
              </span>
              <button on:click={() => clicking(subValue, false)}
                ><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 352 512"
                  ><path
                    d="M242.72 256l100.07-100.07c12.28-12.28 12.28-32.19 0-44.48l-22.24-22.24c-12.28-12.28-32.19-12.28-44.48 0L176 189.28 75.93 89.21c-12.28-12.28-32.19-12.28-44.48 0L9.21 111.45c-12.28 12.28-12.28 32.19 0 44.48L109.28 256 9.21 356.07c-12.28 12.28-12.28 32.19 0 44.48l22.24 22.24c12.28 12.28 32.2 12.28 44.48 0L176 322.72l100.07 100.07c12.28 12.28 32.2 12.28 44.48 0l22.24-22.24c12.28-12.28 12.28-32.19 0-44.48L242.72 256z" /></svg
                ></button>
            </b>
          {/each}
        {/if}
      {/if}
      <input
        {id}
        {placeholder}
        type="text"
        autocomplete="off"
        bind:this={input}
        bind:value={rawValue}
        on:focus={focusing}
        on:blur={blurring} />
    </div>
    <button
      class:focused
      on:click={() => (focused ? input.blur() : input.focus())}>
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"
        ><path
          d="M207.029 381.476L12.686 187.132c-9.373-9.373-9.373-24.569 0-33.941l22.667-22.667c9.357-9.357 24.522-9.375 33.901-.04L224 284.505l154.745-154.021c9.379-9.335 24.544-9.317 33.901.04l22.667 22.667c9.373 9.373 9.373 24.569 0 33.941L240.971 381.476c-9.373 9.372-24.569 9.372-33.942 0z" /></svg>
      <span />
    </button>
  </div>
  <div class="options">
    <div class="list" class:focused>
      {#key matchesFilter}
        {#each options as option (option[1].key)}
          {#if matches(option)}
            <div
              class="option"
              class:selected={single
                ? value === option[0]
                : value.includes(option[0])}
              on:click={() => clicking(option[0])}>
              {#if matchesFilter !== ""}
                {#each option[1].display as letter}
                  {#if matchesFilter.includes(letter.toLowerCase())}
                    <b>{letter}</b>
                  {:else}
                    <span>{letter}</span>
                  {/if}
                {/each}
              {:else}
                {option[1].display}
              {/if}
            </div>
          {/if}
        {/each}
      {/key}
    </div>
  </div>
</div>

<style>
  .root {
    display: flex;
    flex-direction: column;
    position: relative;
  }

  .inputRow {
    display: flex;
  }

  .selection {
    padding: 0em 0.2em;
    border-bottom: 0.15em solid #969696;
    margin: 0.2em 0em 0em 0em;
    font-size: 1em;
    display: flex;
    flex: 1;
    flex-wrap: wrap;
  }

  .selection.focused {
    border-color: #81d272;
  }

  .selection input {
    border: none;
    outline: none;
    font-size: 1em;
    background: none;
    padding: 0;
    vertical-align: top;
    flex: 1;
    height: 1.7em;
    font-family: Roboto, -apple-system, BlinkMacSystemFont, Segoe UI, Oxygen,
      Ubuntu, Cantarell, Fira Sans, Droid Sans, Helvetica Neue, sans-serif;
  }

  ::-webkit-input-placeholder {
    font-style: italic;
    opacity: 0.5;
  }

  ::-moz-placeholder {
    font-style: italic;
    opacity: 0.5;
  }

  :-ms-input-placeholder {
    font-style: italic;
    opacity: 0.5;
  }

  :-moz-placeholder {
    font-style: italic;
    opacity: 0.5;
  }

  .selection .value + input {
    margin-left: 0.1em;
  }

  .selection .value {
    color: white;
    background: #81d272;
    margin: 0.1em;
    border-radius: 0.2em;
    display: flex;
  }

  .selection .value span {
    padding: 0em 0.2em 0em 0.3em;
  }

  .selection .value button {
    border: none;
    background: none;
    outline: none;
    font-size: 1em;
    cursor: pointer;
    padding: 0em;
    width: 1.5em;
    height: 1.5em;
  }

  .selection .value button svg {
    fill: white;
    vertical-align: bottom;
    width: 100%;
    height: 100%;
  }

  .selection .value button svg:hover {
    fill: #5d9b52;
  }

  .list {
    display: none;
    position: absolute;
    overflow: hidden;
    background: white;
    width: 100%;
    border-radius: 0 0 0.5em 0.5em;
    box-shadow: #afafaf 0em 0.4em 0.4em;
    z-index: 100;
    max-height: 10em;
    overflow-y: scroll;
  }

  .list.focused {
    display: block;
  }

  .list .option {
    padding: 0.1em 0.5em;
  }

  .list .option:hover {
    background: #5d9b52;
    color: white !important;
    cursor: pointer;
  }

  .list .option.selected {
    font-weight: 600;
    color: #81d272;
  }

  .inputRow > button {
    height: 1em;
    border: none;
    background: none;
    outline: none;
    font-size: 2em;
    width: 1em;
    display: flex;
    cursor: pointer;
    padding: 0.2em;
  }

  .inputRow > button svg {
    width: 100%;
    height: 100%;
    transition: transform 0.3s;
    transform: rotate(90deg);
  }

  .inputRow > button.focused svg {
    fill: #81d272;
    transform: rotate(0deg);
  }
</style>
