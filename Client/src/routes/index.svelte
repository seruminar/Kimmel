<script lang="ts">
  import wretch from "wretch";
  import { debounce, replace } from "lodash";
  import jwt_decode from "jwt-decode";
  import { onMount } from "svelte";
  import { fade } from "svelte/transition";
  import type { TransitionConfig } from "svelte/transition";
  import {
    editorLineHighlight,
    editorOverviewRulerBackground,
  } from "monaco-editor/esm/vs/editor/common/view/editorColorRegistry";
  import {
    editorBackground,
    editorInactiveSelection,
    editorSelectionBackground,
    minimapBackground,
    scrollbarShadow,
  } from "monaco-editor/esm/vs/platform/theme/common/colorRegistry";
  import "monaco-editor/min/vs/editor/editor.main.css";
  import type * as monaco from "monaco-editor";

  import { translate, translateReady } from "../shared/stores/translate";
  import Loading from "../shared/components/loading.svelte";
  import { localStorage } from "../shared/stores/localStorage";
  import ObjectView from "../shared/components/objectView.svelte";
  import Overlay from "../shared/components/overlay.svelte";
  import SaveToCloud from "../shared/components/saveToCloud.svelte";

  import { KmlDatabase } from "./_database";
  import type { IKml } from "./_database";
  import translations from "./_resources";

  enum LocalStorageKeys {
    Mode = "Kimmel:Mode",
  }

  const languageId = "kml";
  const editedTemplateName = "";

  let ready: boolean = false;

  let monacoRoot: HTMLDivElement;
  let editor: monaco.editor.IStandaloneCodeEditor;
  let resizeObserver: ResizeObserver;

  let tips: {
    translation: string;
    duration: number;
  }[];
  let tipsTimerId: number;
  let tipPointer = 0;
  let tip: {
    translation: string;
    duration: number;
  };

  let db: KmlDatabase;
  let id: number;
  let kml: string = "";
  let modeStrict: boolean = true;

  let parsedLoading: boolean = false;
  let parsedValid: boolean = false;
  let parsedLabel: string = "";
  let parsedObject: any;

  let openExportKontentOverlay: () => void;
  let overlayLoading: boolean = false;
  let overlayLabel: string = "";
  let overlayObject: any;
  let overlayFinished: boolean = false;

  let projectId: string = "";
  let managementApiKey: string = "";
  let managementApiKeyValid: boolean = false;

  let selectedTemplate: IKml;
  let openSaveTemplateOverlay: () => void;
  let closeSaveTemplateOverlay: () => void;
  let newTemplateName: string = "";
  let updateTemplates: boolean = false;

  $: templates =
    updateTemplates !== undefined &&
    db !== undefined &&
    db.kmls.where("name").notEqual(editedTemplateName).toArray();

  $: modeStore = localStorage<"Strict" | "Loose">(
    LocalStorageKeys.Mode,
    "Strict"
  );
  $: modeStore.set(mode);
  $: mode = $modeStore;

  $: {
    if (modeStrict) {
      mode = "Strict";
    } else {
      mode = "Loose";
    }

    if (ready) {
      parseKml();
    }
  }

  $: defaultTemplates = () => [
    [editedTemplateName, ""],
    [$t`template1Name`, $t`template1`],
    [$t`template2Name`, $t`template2`],
    [$t`template3Name`, $t`template3`],
  ];

  onMount(() => {
    db = new KmlDatabase();

    translateReady.subscribe(async (ready) => {
      if (!ready) {
        return;
      }

      tips = [$t`tips`, $t`tip1`, $t`tip2`, $t`tip3`].map((translation) => ({
        translation,
        duration: translation.length * 120,
      }));

      tipsTimer();

      for (const template of defaultTemplates()) {
        const [templateName, templateKml] = template;

        const item = await db.kmls.where("name").equals(templateName).first();

        if (item === undefined) {
          const itemId = await db.kmls.add({
            name: templateName,
            kml: templateKml,
          });

          if (templateName === editedTemplateName) {
            id = itemId;
            kml = templateKml;
          }
        } else {
          if (templateName === editedTemplateName) {
            id = item.id;
            kml = item.kml;
          } else {
            await db.kmls.update(item.id, {
              name: templateName,
              kml: templateKml,
            });
          }
        }
      }

      updateTemplates = !updateTemplates;

      editor = monaco.editor.create(monacoRoot, {
        value: kml,
        language: languageId,
        tabSize: 2,
      });

      editor.onDidChangeModelContent(async () => {
        kml = editor.getValue();

        try {
          await db.kmls.put({ id, name: editedTemplateName, kml });
        } catch (error) {
          console.log(error);
        }
      });

      resizeObserver = new ResizeObserver(() => editor.layout());
      resizeObserver.observe(monacoRoot);
    });

    configureMonaco();

    ready = true;

    return () => {
      resizeObserver.disconnect();
      clearTimeout(tipsTimerId);
    };
  });

  const configureMonaco = () => {
    monaco.editor.defineTheme(languageId, {
      base: "vs-dark",
      inherit: true,
      rules: [
        {
          token: "comment",
          foreground: "f3c598",
        },
        {
          token: "snippet",
          foreground: "8fd683",
        },
        {
          token: "bracket",
          foreground: "dcc960",
        },
      ],
      colors: {
        [editorBackground]: "#38332e",
        [scrollbarShadow]: "#221e1b",
        [minimapBackground]: "#221e1b",
        [editorOverviewRulerBackground]: "#221e1b",
        [editorInactiveSelection]: "#5d524a",
        [editorSelectionBackground]: "#221e1b",
        [editorLineHighlight]: "#a75b3740",
      },
    });

    monaco.editor.setTheme(languageId);

    monaco.languages.register({ id: languageId });

    monaco.languages.setLanguageConfiguration(languageId, {
      comments: {
        lineComment: "//",
      },
      brackets: [
        ["[", "]"],
        ["(", ")"],
      ],
      autoClosingPairs: [
        { open: "[", close: "]" },
        { open: "(", close: ")" },
      ],
    });

    monaco.languages.setMonarchTokensProvider(languageId, {
      defaultToken: "invalid",
      tokenizer: {
        root: [
          // type
          [/^\s*[\w,]+(?=\[)/, "keyword"],
          // name
          [/^\s*\w+/, "type.identifier"],
          // snippet type
          [/\.\.\.\w+/, "snippet"],
          // whitespace
          { include: "@whitespace" },
          // options
          [/[\w,\-+* \(\)]+?(?=\])/, "number"],
          // brackets
          [/[\[\]]/, "bracket"],
          // label
          [/[\w ]+?/, "string"],
        ],
        whitespace: [
          [/[ \t\r\n]+/, "white"],
          [/\/\*/, "comment", "@comment"],
          [/\/\/.*$/, "comment"],
        ],
        comment: [
          [/[^\/*]+/, "comment"],
          [/\/\*/, "comment", "@push"], // nested comment
          ["\\*/", "comment", "@pop"],
          [/[\/*]/, "comment"],
        ],
      },
    });

    monaco.languages.registerCompletionItemProvider(languageId, {
      provideCompletionItems: (model, position, context) => {
        const wordInfo = model.getWordUntilPosition(position);
        const range = new monaco.Range(
          position.lineNumber,
          wordInfo.startColumn,
          position.lineNumber,
          wordInfo.endColumn
        );

        const getInsertText = (id: string) => id + "[${1}] ${2:label}\n$0";

        const propertySuggestions = [
          {
            label: "asset",
            id: "Asset",
            documentation: "Single Asset property",
          },
          {
            label: "text",
            id: "Text",
            documentation: "Single Text property",
          },
          {
            label: "multipleChoice",
            id: "MultipleChoice",
            documentation: "Single Multiple ahoice property",
          },
          {
            label: "singleChoice",
            id: "SingleChoice",
            documentation: "Single Single choice property",
          },
          {
            label: "date",
            id: "Date",
            documentation: "Single Date property",
          },
          {
            label: "number",
            id: "Number",
            documentation: "Single Number property",
          },
          {
            label: "richText",
            id: "RichText",
            documentation: "Single Rich text property",
          },
        ];

        const suggestions: monaco.languages.CompletionItem[] = [
          {
            label: "type",
            kind: monaco.languages.CompletionItemKind.Class,
            range,
            insertText: [
              "${1:TypeId} ${2:type label}",
              "\t${3:PropertyId}[${4}] ${5:label}",
              "\t$0",
            ].join("\n"),
            insertTextRules:
              monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: "Single type with one property",
          },
          {
            label: "snippet",
            kind: monaco.languages.CompletionItemKind.Property,
            range,
            insertText: "...${1:Identifier}\n$0",
            insertTextRules:
              monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: "Single snippet type property",
          },
          {
            label: "prop",
            kind: monaco.languages.CompletionItemKind.Property,
            range,
            insertText: "${1:Identifier}[${2}] ${3:Label}\n$0",
            insertTextRules:
              monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: "Single property",
          },
        ];

        for (const property of propertySuggestions) {
          suggestions.push({
            label: property.label,
            kind: monaco.languages.CompletionItemKind.Property,
            range,
            insertText: getInsertText(property.id),
            insertTextRules:
              monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: property.documentation,
          });
        }

        const textTokens = model.findMatches(
          /[\w]+/gi,
          true,
          true,
          false,
          null,
          true,
          50
        );

        const textSuggestions: {
          [key: string]: monaco.languages.CompletionItem;
        } = {};

        for (const token of textTokens) {
          const match = token.matches[0];

          if (wordInfo.word === match) {
            continue;
          }

          textSuggestions[match] = {
            label: match,
            kind: monaco.languages.CompletionItemKind.Text,
            range,
            insertText: match,
            insertTextRules:
              monaco.languages.CompletionItemInsertTextRule.KeepWhitespace,
          };
        }

        for (const textSuggestion in textSuggestions) {
          suggestions.push(textSuggestions[textSuggestion]);
        }

        return { suggestions };
      },
    });
  };

  const tipsTimer = () => {
    clearTimeout(tipsTimerId);

    if (tip === undefined) {
      tip = tips[tipPointer];
    }

    return setTimeout(() => {
      const nextTipPointer =
        Math.floor(Math.random() * tips.length) % tips.length;

      tipPointer = nextTipPointer;
      tip = tips[nextTipPointer];

      tipsTimerId = tipsTimer();
    }, tip.duration);
  };

  $: if (selectedTemplate !== undefined) {
    editor.setValue(selectedTemplate.kml);
    selectedTemplate = undefined;
  }

  $: ready && debounceKml(kml);

  const debounceKml = debounce(
    (oldKml: string) => kml === oldKml && parseKml(),
    800
  );

  const parseKml = async () => {
    parsedLabel = "";
    parsedValid = false;

    try {
      const response = await wretch("kimmel/parse")
        .post({
          kml,
          mode,
        })
        .error(400, (errorResponse) => {
          parsedLabel = $t`parsedKmlError`;
          parsedObject = JSON.parse(errorResponse.text);
        })
        .error(404, (errorResponse) => {
          parsedLabel = $t`genericError`;
          parsedObject = JSON.parse(errorResponse.text);
        })
        .json();

      if (response) {
        parsedLabel = $t`parsedKml`;
        parsedObject = response.parsedKml;
        parsedValid = true;
      }
    } catch (error) {
      console.log(error);
    }
  };

  const exportZip = async () => {
    parsedLoading = true;
    parsedLabel = "";

    try {
      const blob = await wretch("kimmel/exportZip")
        .post({
          kml,
          mode,
        })
        .error(500, (errorResponse) => {
          parsedLabel = $t`parsedKmlError`;
          parsedObject = JSON.parse(errorResponse.text);
        })
        .error(404, (errorResponse) => {
          parsedLabel = $t`genericError`;
          parsedObject = JSON.parse(errorResponse.text);
        })
        .blob();

      downloadFile(
        blob,
        getFilename($t`kmlExport`, "", new Date().toLocaleString()),
        "zip"
      );
    } catch (error) {
      console.log(error);
    } finally {
      parsedLoading = false;
    }
  };

  const getFilename = (prefix: string, middle: string, suffix: string) => {
    const sanitize = (value: string) =>
      replace(value, /[\/, ]/g, "_").slice(0, 50);

    return `${sanitize(prefix)}_${sanitize(middle)}_${sanitize(suffix)}`;
  };

  const downloadFile = (blob: Blob, filename: string, extension: string) => {
    const link = document.createElement("a");

    link.href = URL.createObjectURL(blob);
    link.download = `${filename}.${extension}`;
    link.style.visibility = "hidden";

    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);

    URL.revokeObjectURL(link.href);
  };

  const saveNewTemplate = async () => {
    const item = await db.kmls.where("name").equals(newTemplateName).first();

    if (item === undefined) {
      await db.kmls.add({
        name: newTemplateName,
        kml,
      });
    } else {
      await db.kmls.update(item.id, {
        name: newTemplateName,
        kml,
      });
    }

    updateTemplates = !updateTemplates;
    closeSaveTemplateOverlay();
  };

  $: if (managementApiKey !== "") {
    try {
      const parsedToken = jwt_decode<{ project_id: string }>(managementApiKey);

      projectId = parsedToken.project_id.replace(
        /([a-f0-9]{8})([a-f0-9]{4})([a-f0-9]{4})([a-f0-9]{4})([a-f0-9]{12})/,
        "$1-$2-$3-$4-$5"
      );

      managementApiKeyValid = true;
    } catch (error) {
      console.log(error);
    }
  }

  const exportKontent = async () => {
    overlayLoading = true;
    overlayObject = undefined;

    try {
      await wretch("kimmel/exportKontent")
        .post({
          kml,
          managementApiKey,
          mode,
        })
        .error(400, (errorResponse) => {
          overlayLabel = $t`parsedKmlError`;
          overlayObject = JSON.parse(errorResponse.text);
        })
        .error(404, (errorResponse) => {
          overlayLabel = $t`genericError`;
          overlayObject = JSON.parse(errorResponse.text);
        })
        .res((response) => {
          if (response.ok) {
            overlayLabel = "";
            overlayObject = undefined;
            overlayFinished = true;
          }
        });
    } catch (error) {
      console.log(error);
    } finally {
      overlayLoading = false;
    }
  };

  const pulse = (node: HTMLElement) => {
    const animationElement = document.createElement("animation");

    node.insertBefore(animationElement, node.firstChild);

    node.style.position = "relative";

    animationElement.style.borderRadius = "inherit";
    animationElement.style.background = "inherit";

    animationElement.style.position = "absolute";
    animationElement.style.left = "50%";
    animationElement.style.top = "50%";
    animationElement.style.width = "100%";
    animationElement.style.height = "100%";
    animationElement.style.opacity = "0";
    animationElement.style.transform = "translate(-50%, -50%)";
    animationElement.style.zIndex = "-1";

    const animateFirstElementChild = () => {
      animationElement.animate(
        [
          {
            transform: "translate(-50%, -50%)",
            opacity: 0.9,
          },
          {
            transform: `translate(-50%, -50%) scale(1.5)`,
            opacity: 0,
          },
        ],
        300
      );
    };

    node.addEventListener("click", animateFirstElementChild);

    return {
      destroy() {
        node.removeEventListener("click", animateFirstElementChild);
      },
    };
  };

  const fadeInPlace: (
    node: HTMLElement,
    options: Partial<{ delay: number; duration: number }>
  ) => TransitionConfig = (node, { delay = 0, duration = 2000 }) => {
    node.parentElement.style.position = "relative";

    const startingOpacity = +getComputedStyle(node).opacity;

    return {
      delay,
      duration,
      css: (timePassed) => {
        return `position:absolute; opacity: ${timePassed * startingOpacity}`;
      },
      tick: (timePassed, timeRemaining) => {
        if (timePassed === 0 || timeRemaining === 0) {
          const height = node.getBoundingClientRect().height;

          node.style.height = `${height}px`;
        }
      },
    };
  };

  const t = translate(translations);
</script>

<svelte:head>
  <title>{$t`kimmel`}</title>
  <script>
    var require = {
      paths: { vs: "https://cdn.jsdelivr.net/npm/monaco-editor@0.22.3/min/vs" },
    };
  </script>
  <script
    src="https://cdn.jsdelivr.net/npm/monaco-editor@0.22.3/min/vs/loader.js"></script>
  <script
    src="https://cdn.jsdelivr.net/npm/monaco-editor@0.22.3/min/vs/editor/editor.main.nls.js"></script>
  <script
    src="https://cdn.jsdelivr.net/npm/monaco-editor@0.22.3/min/vs/editor/editor.main.js"></script>
</svelte:head>

{#if ready}
  <h1>
    {$t`kimmel`}
  </h1>
{:else}
  <h1>&nbsp;</h1>
{/if}
<section>
  {#if ready}
    <div class="overlayContext">
      <Overlay
        bind:openOverlay={openSaveTemplateOverlay}
        bind:closeOverlay={closeSaveTemplateOverlay}
        openOverlayCallback={() => {
          newTemplateName = "";
        }}>
        <div class="item">
          <h4>{$t`saveTemplate`}</h4>
          <p>
            {$t`saveTemplateDescription`}
          </p>
        </div>
        <div class="group item">
          <label class="group column item">
            <span class="item">
              {$t`newTemplateName`}
            </span>
            <input type="text" class="item" bind:value={newTemplateName} />
          </label>
        </div>
        {#if newTemplateName !== ""}
          <br />
          {#await db.kmls.where("name").equals(newTemplateName).toArray()}
            <div class="group item" />
          {:then existingTemplates}
            <div class="group item">
              {#if existingTemplates.length > 0}
                {#if defaultTemplates().some((template) => template[0] === newTemplateName)}
                  <div class="item">
                    <span>
                      {$t`newTemplateNameExistsAsDefault`}
                    </span>
                  </div>
                {:else}
                  <div class="item">
                    <span>
                      {$t`newTemplateNameExists`}
                    </span>
                  </div>
                  <div>
                    <button on:click={saveNewTemplate}
                      >{$t`saveNewTemplate`}</button>
                  </div>
                {/if}
              {:else}
                <div class="item" />
                <div>
                  <button on:click={saveNewTemplate}
                    >{$t`saveNewTemplate`}</button>
                </div>
              {/if}
            </div>
          {/await}
        {/if}
      </Overlay>
      <Overlay
        bind:openOverlay={openExportKontentOverlay}
        openOverlayCallback={() => {
          managementApiKey = "";
          overlayFinished = false;
          overlayObject = undefined;
          managementApiKeyValid = false;
        }}>
        {#if overlayLoading}
          <div class="overlay" out:fade><Loading /></div>
        {/if}
        {#if !overlayFinished}
          <div class="item">
            <h4>{$t`exportIntoProject`}</h4>
            <p>
              {$t`exportDescription`}
            </p>
          </div>
          <div class="group item">
            <label class="group column item">
              <span class="item">
                {$t`managementApiKey`}
              </span>
              <input
                type="password"
                class="item"
                bind:value={managementApiKey} />
            </label>
          </div>
          {#if managementApiKeyValid}
            <br />
            <div class="group item">
              <label class="group checkboxLabel">
                <span>
                  {$t`strict`}
                </span>
                <input type="checkbox" bind:checked={modeStrict} />
                <div class="checkbox" />
              </label>
              <div class="item" />
              <button on:click={exportKontent}>{$t`exportKontent`}</button>
            </div>
          {/if}
        {:else}
          <div class="item">
            <h4>{$t`exported`}</h4>
          </div>
          <div class="item">
            <p>{$t`exportSuccess`}</p>
            <p>
              <a
                class="badge"
                href={`https://app.kontent.ai/${projectId}/content-models/types`}
                target="_blank">{$t`openTypes`}</a>
            </p>
          </div>
        {/if}
        {#if overlayObject}
          <br />
          <ObjectView
            label={overlayLabel}
            object={overlayObject}
            backgroundHover={"#a75b3740"}
            checkedColor={"#a75b37"}
            color={"#6e4f40"} />
        {/if}
      </Overlay>
    </div>
  {/if}
  <div class="kml">
    <div class="sticky">
      <div class="group column">
        <div class="overlayContext">
          <div class="monacoRoot" bind:this={monacoRoot} />
          {#if parsedLoading}
            <div class="overlay" out:fade><Loading /></div>
          {/if}
          {#if kml === "" && editor}
            <div class="overlay block group">
              <span class="item" />
              <div class="group">
                <span class="item" />
                <span>
                  {$t`kmlInTheBox`}
                </span>
                <span class="item" />
              </div>
              <span class="item" />
            </div>
          {/if}
        </div>
        <br />
        <div class="group">
          {#if db !== undefined}
            <label class="group">
              <!-- svelte-ignore a11y-no-onchange -->
              <select
                bind:value={selectedTemplate}
                on:change={(event) => {
                  event.currentTarget.value = "";
                }}>
                <option value={undefined}>{$t`selectTemplate`}</option>
                {#key templates}
                  {#await templates then templates}
                    {#each templates as template (template.id)}
                      <option value={template}>{template.name}</option>
                    {/each}
                  {/await}
                {/key}
              </select>
            </label>
          {/if}
          {#if kml !== ""}
            <button use:pulse on:click={openSaveTemplateOverlay}>
              <div class="svgContext">
                <SaveToCloud />
              </div>
            </button>
            <div class="item" />
            <label use:pulse class="group checkboxLabel">
              <span>
                {$t`strict`}
              </span>
              <input type="checkbox" bind:checked={modeStrict} />
              <div class="checkbox" />
            </label>
            <button use:pulse on:click={exportZip} disabled={!parsedValid}>
              {$t`exportZip`}
            </button>
            <button
              use:pulse
              on:click={openExportKontentOverlay}
              disabled={!parsedValid}>{$t`exportKontent`}</button>
          {/if}
        </div>
      </div>
      <div class="group column">
        <br />
        <h2>{$t`tipsHeading`}</h2>
        {#if tip}
          <div>
            {#key tip}
              <div class="tip item" transition:fadeInPlace={{ duration: 500 }}>
                <p>
                  {@html tip.translation}
                </p>
              </div>
            {/key}
          </div>
        {/if}
      </div>
      <div />
    </div>
  </div>
  <div class="result group column">
    {#if parsedObject}
      <ObjectView
        label={parsedLabel}
        checked={true}
        object={parsedObject}
        backgroundHover={"#a75b3740"}
        checkedColor={"#a75b37"}
        color={"#6e4f40"}
        getObjectLabel={(object) => {
          if (object.id !== undefined) {
            return object.id;
          } else if (object.label !== undefined) {
            return object.label;
          }

          return "";
        }}
        sortObjectKeys={(key) => {
          switch (key) {
            case "id":
              return 1;
            case "type":
              return 2;
            case "label":
              return 3;
            case "required":
              return 4;
          }
          return 100;
        }} />
    {/if}
  </div>
</section>

<style>
  :global(main) {
    background: linear-gradient(135deg, hsl(22deg, 9%, 98%), hsl(19deg 50% 70%))
      fixed;
    position: relative;
    color: #a75b37;
  }

  .group {
    display: flex;
  }

  .column {
    flex-direction: column;
  }

  .item {
    flex: 1;
  }

  h1 {
    font-size: calc((1vh + 1vw) * 2.5);
    text-transform: uppercase;
    font-weight: 1000;
    margin: 0 0 calc((1vh + 1vw) * 0.2) calc((1vh + 1vw) * 1.5);
  }

  .tip p {
    margin: 0;
  }

  .tip :global(code) {
    font-weight: 600;
  }

  .tip :global(sup) {
    display: inline-block;
    border-style: solid;
    color: #a75b37;
    border-color: #a75b37;
    font-weight: 600;
    padding: calc((1vh + 1vw) * 0.05) calc((1vh + 1vw) * 0.1);
    font-size: 0.85em;
    border-width: calc((1vh + 1vw) * 0.1);
    border-radius: calc((1vh + 1vw) * 0.2);
    vertical-align: initial;
    line-height: 1.1em;
  }

  section {
    flex-direction: row;
    margin: 0 0 0 calc((1vh + 1vw) * 1.5);
  }

  .monacoRoot {
    min-height: calc((1vh + 1vw) * 20);
    border-radius: calc((1vh + 1vw) * 0.5);
    overflow: hidden;
  }

  .kml {
    display: flex;
    flex-direction: column;
    flex: 1;
    min-width: calc((1vh + 1vw) * 15);
  }

  .kml .sticky {
    position: sticky;
    top: calc((1vh + 1vw) * 1.5);
    display: flex;
    flex-direction: column;
  }

  .overlayContext {
    position: relative;
  }

  .overlay {
    position: absolute;
    left: 0;
    top: 0;
    height: 100%;
    width: 100%;
    place-content: center;
    display: flex;
    flex-direction: column;
  }

  .overlay.block {
    pointer-events: none;
  }

  .kml .monacoRoot + .overlay {
    font-size: calc((1vh + 1vw) * 1.5);
    font-weight: 800;
    color: hsl(22deg, 9%, 48%);
  }

  label {
    color: #a75b37;
    border: #a75b37 calc((1vh + 1vw) * 0.05) solid;
    font-weight: 700;
    border-radius: calc((1vh + 1vw) * 0.5) / calc((1vh + 1vw) * 1);
    font-size: calc((1vh + 1vw) * 0.6);
    place-items: center;
    transition: 0.4s cubic-bezier(0, 0, 0, 1);
    text-transform: uppercase;
    min-height: calc((1vh + 1vw) * 1.5);
  }

  label:hover {
    box-shadow: #a75b37 0 calc((1vh + 1vw) * 0.1) calc((1vh + 1vw) * 1);
    background: hsl(19deg 74% 78%);
  }

  label span {
    margin: 0 0 0 calc((1vh + 1vw) * 0.5);
  }

  button {
    border: none;
    outline: none;
    background: none;
    border: #a75b37 calc((1vh + 1vw) * 0.05) solid;
    height: calc((1vh + 1vw) * 1.5);
    padding: 0 calc((1vh + 1vw) * 0.5);
    font-size: calc((1vh + 1vw) * 0.6);
    font-weight: 700;
    border-radius: calc((1vh + 1vw) * 0.5) / calc((1vh + 1vw) * 1);
    color: #a75b37;
    fill: #a75b37;
    margin: 0 0 0 calc((1vh + 1vw) * 0.5);
    font-family: inherit;
    transition: 0.4s cubic-bezier(0, 0, 0, 1);
    text-transform: uppercase;
  }

  button:focus {
    outline: none;
  }

  button:hover:not(:disabled) {
    box-shadow: #a75b37 0 calc((1vh + 1vw) * 0.1) calc((1vh + 1vw) * 1);
    background: hsl(19deg 74% 78%);
    cursor: pointer;
  }

  button:disabled {
    border-color: hsl(21deg 9% 48%);
    background: hsl(21deg 9% 48%);
    color: hsla(22deg, 9%, 98%, 55%);
    cursor: not-allowed;
  }

  .svgContext {
    height: 100%;
    padding: calc((1vh + 1vw) * 0.2);
  }

  select {
    flex: 1;
    outline: none;
    color: #a75b37;
    font-size: calc((1vh + 1vw) * 0.6);
    font-family: inherit;
    font-weight: 600;
    border-radius: calc((1vh + 1vw) * 0.5) / calc((1vh + 1vw) * 1);
    background: none;
    height: 100%;
    border: none;
    margin: 0 calc((1vh + 1vw) * 0.5);
    transition: 0.4s cubic-bezier(0, 0, 0, 1);
  }

  select:hover {
    background: hsl(19deg 74% 78%);
  }

  input[type="checkbox"] {
    display: none;
  }

  input[type="checkbox"] + .checkbox {
    height: calc((1vh + 1vw) * 1);
    width: calc((1vh + 1vw) * 1);
    background: none;
    border: #a75b37 calc((1vh + 1vw) * 0.05) solid;
    border-radius: calc((1vh + 1vw) * 0.2);
    margin: 0 calc((1vh + 1vw) * 0.5);
  }

  input[type="checkbox"]:hover:not(:checked) + .checkbox {
    background: #b18b7a !important;
  }

  input[type="checkbox"]:hover + .checkbox {
    background: #855e4b !important;
  }

  input[type="checkbox"]:checked + .checkbox {
    background: #7c523f;
  }

  .checkboxLabel {
    margin: 0 0 0 calc((1vh + 1vw) * 0.5);
  }

  .checkboxLabel:hover {
    cursor: pointer;
  }

  input[type="password"],
  input[type="text"] {
    outline: none;
    background: none;
    border: none;
    font-size: calc((1vh + 1vw) * 0.8);
    width: 100%;
    height: calc((1vh + 1vw) * 1);
    padding: calc((1vh + 1vw) * 0.5);
    font-family: inherit;
  }

  input[type="password"]:focus,
  input[type="text"]:focus {
    border-color: #a75b37;
  }

  .result {
    flex: 0.5;
    padding: 0 0 calc((1vh + 1vw) * 1) calc((1vh + 1vw) * 0.5);
    color: #6e4f40;
  }
</style>
