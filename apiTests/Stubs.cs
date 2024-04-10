using SO_tags.Models;

namespace SO_tagsTests;

public static class Stubs
{
    public const string StackExchangeResponse =
      "{\n  \"items\": [\n    {\n      \"synonyms\": [\n        \"js\",\n        \"ecmascript\",\n        \".js\",\n        \"javascript-execution\",\n        \"classic-javascript\",\n        \"javascript-alert\",\n        \"javascript-dom\",\n        \"javascript-disabled\",\n        \"javascript-library\",\n        \"javascript-runtime\",\n        \"vanilla-javascript\",\n        \"javascript-module\",\n        \"vanilla-js\",\n        \"vanillajs\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 2529031,\n      \"name\": \"javascript\"\n    },\n    {\n      \"synonyms\": [\n        \"pythonic\",\n        \"python-interpreter\",\n        \"python-shell\",\n        \"py\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 2192572,\n      \"name\": \"python\"\n    },\n    {\n      \"synonyms\": [\n        \"java-se\",\n        \".java\",\n        \"j2se\",\n        \"core-java\",\n        \"jdk\",\n        \"jre\",\n        \"java-libraries\",\n        \"oraclejdk\",\n        \"openjdk\",\n        \"javax\",\n        \"java-api\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 1917388,\n      \"name\": \"java\"\n    },\n    {\n      \"synonyms\": [\n        \"c-sharp\",\n        \"c#.net\",\n        \"c#-language\",\n        \"visual-c#\",\n        \"csharp\",\n        \".cs-file\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 1615172,\n      \"name\": \"c#\"\n    },\n    {\n      \"synonyms\": [\n        \"php-oop\",\n        \"php-date\",\n        \"php5\",\n        \"php-frameworks\",\n        \"hypertext-preprocessor\",\n        \"php.ini\",\n        \"php-cli\",\n        \"php-errors\",\n        \"php-mail\",\n        \"php-cgi\",\n        \"php-functions\",\n        \"php-session\",\n        \"php-fpm\",\n        \"phtml\",\n        \"php-include\",\n        \"php-namespaces\",\n        \"phpinfo\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 1464504,\n      \"name\": \"php\"\n    },\n    {\n      \"synonyms\": [\n        \"android-mobile\",\n        \"android-sdk\",\n        \"android-api\",\n        \"android-device\",\n        \"android-application\",\n        \"android-ui\",\n        \"android-framework\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 1417286,\n      \"name\": \"android\"\n    },\n    {\n      \"synonyms\": [\n        \"html-tag\",\n        \"html-attributes\",\n        \"div\",\n        \"divs\",\n        \"nested-divs\",\n        \"div-layouts\",\n        \"html-comments\",\n        \"span\",\n        \"webpage\",\n        \"time-tag\",\n        \"html5\",\n        \"html-head\",\n        \"html-syntax\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 1187385,\n      \"name\": \"html\"\n    },\n    {\n      \"synonyms\": [\n        \"jquery-core\",\n        \"jquery-live\",\n        \"addclass\",\n        \"toggleclass\",\n        \"removeclass\",\n        \"jquery-callback\",\n        \"jquery-after\",\n        \"jquery-hasclass\",\n        \"jquery-get\",\n        \"jquery-post\",\n        \"jquery-find\",\n        \"jquery-css\",\n        \"jquery-filter\",\n        \"jquery-effects\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 1034800,\n      \"name\": \"jquery\"\n    },\n    {\n      \"synonyms\": [\n        \"cpp\",\n        \"cxx\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 806787,\n      \"name\": \"c++\"\n    },\n    {\n      \"synonyms\": [\n        \"cascading-style-sheet\",\n        \"css-layout\",\n        \"css-background-image\",\n        \"css-attributes\",\n        \"css-classes\",\n        \"css-height\",\n        \"css2\",\n        \"css-columns\",\n        \"css-inheritance\",\n        \"css-centering\",\n        \"min-height\",\n        \"min-width\",\n        \"max-height\",\n        \"max-width\",\n        \"box-model\",\n        \"style.css-template-file\",\n        \"css-validation\",\n        \"css-display\",\n        \"css-line-height\",\n        \"css-overflow\",\n        \"alternate-stylesheets\",\n        \"inline-block\",\n        \"css-box-model\",\n        \"font-weight\",\n        \"css-menu\",\n        \"css-reset\",\n        \"css-borders\",\n        \"dynamic-css\",\n        \"css-font-weight\",\n        \"css3\",\n        \"css-text-shadow\",\n        \"css-box-shadow\",\n        \"css-border-radius\",\n        \"css-border-image\",\n        \"css-text-overflow\",\n        \"box-sizing\",\n        \"box-shadow\"\n      ],\n      \"has_synonyms\": true,\n      \"is_moderator_only\": false,\n      \"is_required\": false,\n      \"count\": 804263,\n      \"name\": \"css\"\n    }\n  ],\n  \"has_more\": false,\n  \"quota_max\": 10000,\n  \"quota_remaining\": 9982\n}";

    public static readonly List<Tag> TagsInDb =
    [
      new Tag()
    {
      Name = "javascript",
      Count = 2529030,
      HasSynonyms = true,
      IsModeratorOnly = false,
      IsRequired = false,
      Synonyms =
      [
        "vanilla-javascript", "javascript-module", "vanilla-js", "vanillajs",
        "javascript-disabled", "javascript-alert", "javascript-dom",
        "javascript-library", "javascript-runtime", "classic-javascript",
        "ecmascript", "js", ".js", "javascript-execution"
      ],
      ShareDescPosition = 1
    },
    new Tag()
    {
      Name = "python",
      Count = 2192571,
      HasSynonyms = true,
      IsModeratorOnly = false,
      IsRequired = false,
      Synonyms =
      [
        "py", "pythonic", "python-interpreter", "python-shell"
      ],
      ShareDescPosition = 2

    },
    new Tag()
    {
      Name = "java",
      Count = 1917389,
      HasSynonyms = true,
      IsModeratorOnly = false,
      IsRequired = false,
      Synonyms =
      [
        "oraclejdk", "javax", "openjdk", "java-api", "java-se", ".java", "j2se",
        "core-java", "java-libraries", "jdk", "jre"
      ],
      ShareDescPosition= 3
    },
  ];
}