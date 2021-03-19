import type { Resource, TFunction } from "i18next";
import i18next from 'i18next';
import { BehaviorSubject } from 'rxjs';
import { readable } from 'svelte/store';

export const translateReady = new BehaviorSubject<boolean>(false);

let initFinished = false;

export const translate = (
  translations: Resource | Resource[],
  locale = "en_us"
) =>
  readable<TFunction>(
    () => {
      if (!initFinished) {
        return "";
      } else {
        loadTranslations(translations);

        return i18next.t.bind(i18next);
      }
    },
    (set) => {
      if (!initFinished) {
        initI18Next(locale, (t) => {
          loadTranslations(translations);
          set(t);
        });
      } else {
        loadTranslations(translations);

        set(i18next.t.bind(i18next));
      }
    }
  );

async function initI18Next(locale: string, loaded: (value: TFunction) => void) {
  const t = await i18next.init({
    lng: locale,
  });

  loaded(t);

  if (!initFinished) {
    translateReady.next(true);
    initFinished = true;
  }
}

function loadTranslations(translations: Resource | Resource[]) {
  let arrayTranslations: Resource[];

  if (Array.isArray(translations)) {
    arrayTranslations = translations;
  } else {
    arrayTranslations = [translations];
  }

  for (const translation of arrayTranslations)
    for (const languageName in translation) {
      const language = translation[languageName];

      for (const namespaceName in language) {
        const resouceBundle = language[namespaceName];

        i18next.addResourceBundle(
          languageName,
          namespaceName,
          resouceBundle,
          true
        );
      }
    }
}
