function init() {

    monaco.languages.register({ id: "serial" });

    monaco.languages.setMonarchTokensProvider("serial", {
        tokenizer: {
            root: [
                [/\[error.*/, "custom-error"],
                [/\[accept.*/, "custom-notice"],
                [/\[send.*/, "custom-info"],
                [/\[[a-zA-Z 0-9:]+\]/, "custom-date"],
            ],
        },
    });
    monaco.languages.registerCompletionItemProvider("serial", {
        provideCompletionItems: (model, position) => {
            var word = model.getWordUntilPosition(position);
            var range = {
                startLineNumber: position.lineNumber,
                endLineNumber: position.lineNumber,
                startColumn: word.startColumn,
                endColumn: word.endColumn,
            };
            var suggestions = [
                {
                    label: "simpleText",
                    kind: monaco.languages.CompletionItemKind.Text,
                    insertText: "simpleText",
                    range: range,
                },
                {
                    label: "testing",
                    kind: monaco.languages.CompletionItemKind.Keyword,
                    insertText: "testing(${1:condition})",
                    insertTextRules:
                        monaco.languages.CompletionItemInsertTextRule
                            .InsertAsSnippet,
                    range: range,
                },
                {
                    label: "ifelse",
                    kind: monaco.languages.CompletionItemKind.Snippet,
                    insertText: [
                        "if (${1:condition}) {",
                        "\t$0",
                        "} else {",
                        "\t",
                        "}",
                    ].join("\n"),
                    insertTextRules:
                        monaco.languages.CompletionItemInsertTextRule
                            .InsertAsSnippet,
                    documentation: "If-Else Statement",
                    range: range,
                },
            ];
            return { suggestions: suggestions };
        },
    });

    monaco.editor.defineTheme("serialTheme", {
        base: "vs",
        inherit: false,
        rules: [
            { token: "custom-info", foreground: "0292fb" },
            { token: "custom-error", foreground: "ff0000", fontStyle: "bold" },
            { token: "custom-notice", foreground: "FFA500" },
            { token: "custom-date", foreground: "008800" },
        ],
        colors: {
            "editor.foreground": "#000000",
        },
    });

}

export {
    init
}