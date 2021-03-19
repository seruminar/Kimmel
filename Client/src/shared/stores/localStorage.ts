import { writable } from 'svelte/store';

export const localStorage = <T>(key: string, defaultValue?: T) => {
  if (typeof window !== "undefined") {
    if (
      defaultValue === undefined ||
      window.localStorage.getItem(key) !== null
    ) {
      defaultValue = JSON.parse(window.localStorage.getItem(key));
    }

    window.addEventListener(
      "storage",
      (event) => event.key === key && set(JSON.parse(event.newValue))
    );
  }

  const { subscribe, set } = writable(defaultValue);

  return {
    subscribe,
    set: (value: T) =>
      typeof window !== "undefined" &&
      window.localStorage.setItem(key, JSON.stringify(value)),
    update: (updater: (value: T) => T) => {
      if (typeof window !== "undefined") {
        const value = JSON.parse(window.localStorage.getItem(key));

        set(updater(value));
      }
    },
  };
};
