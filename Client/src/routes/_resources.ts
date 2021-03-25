export default {
  en_us: {
    translation: {
      kimmel: "Kimmel",
      kmlInTheBox: "Write KML here...",
      strict: "Strict",
      parse: "Parse",
      exportZip: "Export to ZIP",
      exportKontent: "Export to Kontent",
      parsedKml: "Parsed KML JSON",
      genericError: "Error",
      parsedKmlError: "Parsing error",
      exportIntoProject: "Export your KML into a Kontent project",
      exportDescription:
        "Create content types according to the KML schema, if it is valid.",
      managementApiKey: "Management API key",
      exported: "KML successfully exported into the Kontent project",
      exportSuccess:
        "The content models you have defined are now in your project! Check them out by clicking the link below.",
      openTypes: "Open Content model in Kontent",
      tipsHeading: "Kimmel's tips",
      tips: "Watch this space for useful tips on writing KML.",
      tip1:
        "<b>Snippets</b>: Try writing <code>type</code> in an open space in the editor, and then press <sup>Tab</sup>. This inserts a <i>snippet</i> with insert points you can cycle through and fill out. Other snippets are <code>snippet</code>, <code>prop</code>, and one for each property type.",
      tip2:
        "<b>Types</b>: A KML type is defined as a property without options (the characters <code>[</code> and <code>]</code> and anything in between them). Types have an identifier and can optionally have a label. If the mode is <i>Strict</i>, the KML will not parse if a type is linked or used as a snippet and is not described.",
      tip3:
        "<b>Monaco</b>: Kimmel's editor is called Monaco. To see a list of available actions, press <sup>F1</sup> while inside the editor.",
      selectTemplate: "Load a template...",
      template1Name: "Blog",
      template1:
        "Blog Blog\n  Text[characters(60),*] Name\n  Post[1+] Featured posts\n  Social[1+] Social profiles\n\nPost Blog post\n  Text[characters(80),*] Title\n  Text[*] Teaser\n  Person[1+,*] Authors\n  CreditedImage[1,*] Header\n  Category[1+] Category\n  RichText[p,b,i,a,ul,images,h2,*] Body\n    Product\n\nPerson\n  Text[words(3),*] Name\n  Text[] URL\n  Asset[1,images] Photo\n  SingleChoice[Contributor,Associate,Owner] Position\n  RichText[p,a,words(30)] Short biography\n  RichText[p,a,words(60)] Biography\n  Social[] Social profiles\n\nCreditedImage\n  Asset[images,*] Header\n  Person[1+,*] Authors\n\nCategory\n  Text[words(3)] Name\n  MultipleChoice[Highlight] Options\n\nProduct\n  Text[*] Name\n  Number[*] Price\n  Number[*] Discount\n  Text[*] Store name\n  Asset[1,images,*] Photo\n  RichText[p,a,words(60)] Description\n  Text[*] URL\n\nSocial\n  Text[*] URL\n  Asset[images] Icon",
      template2Name: "Billboard",
      template2:
        "Billboard Billboard\n  Ad[1+,*] Ads\n  Transition[1] Default transition\n\nAd\n  Asset[images,1,*] Content\n  Number[*] Duration in seconds\n  SingleChoice[Preferred,High,Medium,Low,Filler,*] Priority\n  Transition[1] Custom transition\n\nTransition\n  Text[words(5),*] Name\n  RichText[p,b,i,a] Description",
      template3Name: "Todo list",
      template3:
        "TodoList Todo list\n  Project[1+,*] Projects\n\nProject\n  Text[*] Name\n  RichText[p,a,b,i] Notes\n  TodoItem[] Todo items\n\nTodoItem Todo item\n  RichText[p,a,b,i,*] Content\n  Date[] Due date\n  Text[] Comment\n  Reminder[] Reminders\n  SingleChoice[Not started,In progress,Completed] Status\n  SingleChoice[Red,Blue,Green,Orange,Yellow,Purple] Label\n  TodoItem[] Todo items\n  TodoTask[] Tasks\n  \nReminder\n  SingleChoice[Email,Text message,App] Type\n  MultipleChoice[Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,*] Days\n  Date[*] Time\n  SingleChoice[Once,Weekly,*] Frequency\n  Number[] Maximum times\n\nTodoTask\n  RichText[p,a,b,i,*] Content\n  MultipleChoice[Highlight,Optional] Options",
      saveTemplate: "Save KML as a new template",
      saveTemplateDescription:
        "Save the KML you have as a template with a custom name so that you can load it later.",
      newTemplateName: "New template name",
      saveNewTemplate: "Save new template",
      newTemplateNameExists:
        "That template name already exists! If you proceed, you will save over the existing template.",
      newTemplateNameExistsAsDefault:
        "That template name already exists as a default template. You cannot save over default templates.",
    },
  },
};
