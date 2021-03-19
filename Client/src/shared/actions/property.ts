import { is_function } from "svelte/internal";

type ActionType<T = any> = [string, T] | [string, () => T];

export const property = (node: HTMLElement, target: ActionType) => {
  setProperty(target, node);

  return {
    update: (target: ActionType) => {
      setProperty(target, node);
    },
  };
};

function setProperty(target: ActionType<any>, node: HTMLElement) {
  let [name, value] = extractValue(target);

  if (value !== undefined) {
    node.style.setProperty(`--${name}`, String(value));
  }
}

const extractValue = (target: ActionType) => {
  let [name, value] = target;

  if (is_function(value)) {
    return [name, value()];
  }

  return [name, value];
};
