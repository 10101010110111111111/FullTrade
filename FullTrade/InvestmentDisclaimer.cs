using System;
using System.Collections.Generic;

namespace FullTrade
{
    public class InvestmentDisclaimer
    {
        public class RiskStatement
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public bool RequiresAcknowledgment { get; set; }
        }

        private readonly List<RiskStatement> _riskStatements;

        public InvestmentDisclaimer()
        {
            _riskStatements = InitializeRiskStatements();
        }

        private List<RiskStatement> InitializeRiskStatements()
        {
            return new List<RiskStatement>
            {
                new RiskStatement
                {
                    Title = "Riziko ztráty investice",
                    Description = "Investování na finančních trzích je spojeno s rizikem ztráty části nebo celé investované částky. " +
                                "Historické výnosy nejsou zárukou budoucích výnosů. Hodnota investice může v čase jak růst, tak klesat.",
                    RequiresAcknowledgment = true
                },
                new RiskStatement
                {
                    Title = "Tržní riziko",
                    Description = "Ceny finančních instrumentů mohou významně kolísat v důsledku různých tržních faktorů včetně " +
                                "ekonomických, politických a dalších událostí. Tyto výkyvy mohou vést k významným ztrátám.",
                    RequiresAcknowledgment = true
                },
                new RiskStatement
                {
                    Title = "Měnové riziko",
                    Description = "Při investování do instrumentů v cizí měně existuje riziko ztráty způsobené změnami měnových kurzů. " +
                                "Nepříznivý vývoj měnového kurzu může negativně ovlivnit celkový výnos investice.",
                    RequiresAcknowledgment = true
                },
                new RiskStatement
                {
                    Title = "Likvidní riziko",
                    Description = "Některé investiční nástroje mohou být obtížně prodejné nebo jejich prodej může být spojen " +
                                "s dodatečnými náklady či časovou prodlevou.",
                    RequiresAcknowledgment = false
                },
                new RiskStatement
                {
                    Title = "Kreditní riziko",
                    Description = "Existuje riziko, že emitent finančního nástroje nebude schopen dostát svým závazkům " +
                                "(např. vyplatit úroky nebo jistinu).",
                    RequiresAcknowledgment = false
                },
                new RiskStatement
                {
                    Title = "Riziko pákového efektu",
                    Description = "Použití pákového efektu může znásobit jak zisky, tak ztráty. Obchodování s pákovým efektem " +
                                "je vysoce rizikové a může vést ke ztrátě přesahující počáteční investici.",
                    RequiresAcknowledgment = true
                },
                new RiskStatement
                {
                    Title = "Odpovědné investování",
                    Description = "Nikdy neinvestujte více, než si můžete dovolit ztratit. Doporučujeme investovat pouze volné " +
                                "prostředky a mít vytvořenou dostatečnou finanční rezervu pro nenadálé situace.",
                    RequiresAcknowledgment = true
                },
                new RiskStatement
                {
                    Title = "Diverzifikace",
                    Description = "Je důležité neinvestovat všechny prostředky do jednoho typu investice. Diverzifikace portfolia " +
                                "pomáhá snižovat investiční riziko.",
                    RequiresAcknowledgment = false
                }
            };
        }

        public class DisclaimerConfirmation
        {
            public bool HasReadDisclaimer { get; set; }
            public bool UnderstandsRisks { get; set; }
            public bool AcceptsResponsibility { get; set; }
            public DateTime ConfirmationDate { get; set; }
            public string InvestorName { get; set; }
            public string InvestorId { get; set; }
        }

        public List<RiskStatement> GetRiskStatements()
        {
            return _riskStatements;
        }

        public bool ValidateConfirmation(DisclaimerConfirmation confirmation)
        {
            if (confirmation == null)
                return false;

            if (string.IsNullOrWhiteSpace(confirmation.InvestorName) ||
                string.IsNullOrWhiteSpace(confirmation.InvestorId))
                return false;

            if (!confirmation.HasReadDisclaimer ||
                !confirmation.UnderstandsRisks ||
                !confirmation.AcceptsResponsibility)
                return false;

            if (confirmation.ConfirmationDate == default)
                return false;

            return true;
        }

        public string GenerateDisclaimerDocument(DisclaimerConfirmation confirmation)
        {
            if (!ValidateConfirmation(confirmation))
                throw new ArgumentException("Neplatné potvrzení disclaimeru");

            var document = new List<string>
            {
                "INVESTIČNÍ PROHLÁŠENÍ A POTVRZENÍ O SEZNÁMENÍ S RIZIKY",
                "========================================================",
                $"\nDatum: {confirmation.ConfirmationDate:dd.MM.yyyy HH:mm}",
                $"Investor: {confirmation.InvestorName}",
                $"ID Investora: {confirmation.InvestorId}",
                "\nSEZNAM RIZIK A UPOZORNĚNÍ:",
                "----------------------------\n"
            };

            foreach (var statement in _riskStatements)
            {
                document.Add($"* {statement.Title}");
                document.Add($"  {statement.Description}\n");
            }

            document.AddRange(new[]
            {
                "POTVRZENÍ INVESTORA:",
                "--------------------",
                "Svým podpisem potvrzuji že:",
                "1. Jsem si přečetl/a všechna výše uvedená upozornění na rizika",
                "2. Rozumím všem uvedeným rizikům a jejich možným dopadům",
                "3. Přijímám odpovědnost za své investiční rozhodnutí",
                "\nPodpis: _______________________",
                $"Datum: {confirmation.ConfirmationDate:dd.MM.yyyy}"
            });

            return string.Join("\n", document);
        }
    }
}