using System;
using System.Collections.Generic;
using System.Linq;

namespace FullTrade
{
    public class RiskProfileQuestion
    {
        public string QuestionText { get; set; }
        public List<Answer> PossibleAnswers { get; set; }
    }

    public class Answer
    {
        public string Text { get; set; }
        public int Score { get; set; }
    }

    public class InvestorProfile
    {
        public string RiskLevel { get; set; }
        public string Description { get; set; }
        public Dictionary<string, double> AssetAllocation { get; set; }
    }

    public class InvestmentQuestionnaire
    {
        private readonly List<RiskProfileQuestion> _questions;
        private readonly Dictionary<string, InvestorProfile> _profiles;

        public InvestmentQuestionnaire()
        {
            _questions = InitializeQuestions();
            _profiles = InitializeProfiles();
        }

        private List<RiskProfileQuestion> InitializeQuestions()
        {
            return new List<RiskProfileQuestion>
    {
        new RiskProfileQuestion
        {
            QuestionText = "Jaký je váš investiční horizont?",
            PossibleAnswers = new List<Answer>
            {
                new Answer { Text = "Méně než 1 rok", Score = 1 },
                new Answer { Text = "1-3 roky", Score = 2 },
                new Answer { Text = "3-7 let", Score = 3 },
                new Answer { Text = "Více než 7 let", Score = 4 }
            }
        },
        new RiskProfileQuestion
        {
            QuestionText = "Jaké jsou vaše zkušenosti s investováním?",
            PossibleAnswers = new List<Answer>
            {
                new Answer { Text = "Žádné", Score = 1 },
                new Answer { Text = "Základní (spořící účty, dluhopisy)", Score = 2 },
                new Answer { Text = "Střední (akcie, podílové fondy)", Score = 3 },
                new Answer { Text = "Pokročilé (deriváty, kryptoměny)", Score = 4 }
            }
        },
        new RiskProfileQuestion
        {
            QuestionText = "Jak byste zareagovali na pokles hodnoty portfolia o 20%?",
            PossibleAnswers = new List<Answer>
            {
                new Answer { Text = "Okamžitě bych vše prodal", Score = 1 },
                new Answer { Text = "Prodal bych část investic", Score = 2 },
                new Answer { Text = "Vyčkal bych a nic nedělal", Score = 3 },
                new Answer { Text = "Dokoupil bych za nižší cenu", Score = 4 }
            }
        },
        new RiskProfileQuestion
        {
            QuestionText = "Jaký je váš měsíční příjem a jakou část můžete investovat?",
            PossibleAnswers = new List<Answer>
            {
                new Answer { Text = "Nemohu pravidelně investovat", Score = 1 },
                new Answer { Text = "Mohu investovat do 10% příjmu", Score = 2 },
                new Answer { Text = "Mohu investovat 10-30% příjmu", Score = 3 },
                new Answer { Text = "Mohu investovat více než 30% příjmu", Score = 4 }
            }
        },
        new RiskProfileQuestion
        {
            QuestionText = "Jaký je váš primární investiční cíl?",
            PossibleAnswers = new List<Answer>
            {
                new Answer { Text = "Ochrana kapitálu", Score = 1 },
                new Answer { Text = "Vyvážený růst s nižším rizikem", Score = 2 },
                new Answer { Text = "Dlouhodobý růst se středním rizikem", Score = 3 },
                new Answer { Text = "Maximální výnos s vysokým rizikem", Score = 4 }
            }
        },
        new RiskProfileQuestion
        {
            QuestionText = "Kolik času chcete věnovat správě svého portfolia?",
            PossibleAnswers = new List<Answer>
            {
                new Answer { Text = "Žádný čas, preferuji pasivní přístup", Score = 1 },
                new Answer { Text = "Minimální čas, stačí mi základní přehled", Score = 2 },
                new Answer { Text = "Pravidelně sleduji trhy a přizpůsobuji strategie", Score = 3 },
                new Answer { Text = "Denně se aktivně věnuji tradingu", Score = 4 }
            }
        },
        new RiskProfileQuestion
        {
            QuestionText = "Jaké riziko jste ochotni akceptovat při investování?",
            PossibleAnswers = new List<Answer>
            {
                new Answer { Text = "Minimální riziko", Score = 1 },
                new Answer { Text = "Nízké riziko", Score = 2 },
                new Answer { Text = "Střední riziko", Score = 3 },
                new Answer { Text = "Vysoké riziko", Score = 4 }
            }
        },
        new RiskProfileQuestion
        {
            QuestionText = "Jaký je váš postoj k investování do nových technologií nebo kryptoměn?",
            PossibleAnswers = new List<Answer>
            {
                new Answer { Text = "Nechci investovat do spekulativních aktiv", Score = 1 },
                new Answer { Text = "Pouze malá část portfolia", Score = 2 },
                new Answer { Text = "Významná část portfolia", Score = 3 },
                new Answer { Text = "Většina portfolia", Score = 4 }
            }
        }
    };
        }

        private Dictionary<string, InvestorProfile> InitializeProfiles()
        {
            return new Dictionary<string, InvestorProfile>
            {
                {
                    "Konzervativní",
                    new InvestorProfile
                    {
                        RiskLevel = "Nízké",
                        Description = "Preferujete stabilitu a ochranu kapitálu před vysokými výnosy.",
                        AssetAllocation = new Dictionary<string, double>
                        {
                            { "Hotovost a krátkodobé instrumenty", 0.30 },
                            { "Dluhopisy", 0.50 },
                            { "Akcie", 0.15 },
                            { "Alternativní investice", 0.05 }
                        }
                    }
                },
                {
                    "Vyvážený",
                    new InvestorProfile
                    {
                        RiskLevel = "Střední",
                        Description = "Hledáte rovnováhu mezi stabilním růstem a přiměřeným rizikem.",
                        AssetAllocation = new Dictionary<string, double>
                        {
                            { "Hotovost a krátkodobé instrumenty", 0.15 },
                            { "Dluhopisy", 0.35 },
                            { "Akcie", 0.40 },
                            { "Alternativní investice", 0.10 }
                        }
                    }
                },
                {
                    "Růstový",
                    new InvestorProfile
                    {
                        RiskLevel = "Vyšší",
                        Description = "Upřednostňujete vyšší výnosy a jste ochotni podstoupit vyšší riziko.",
                        AssetAllocation = new Dictionary<string, double>
                        {
                            { "Hotovost a krátkodobé instrumenty", 0.05 },
                            { "Dluhopisy", 0.20 },
                            { "Akcie", 0.60 },
                            { "Alternativní investice", 0.15 }
                        }
                    }
                },
                {
                    "Dynamický",
                    new InvestorProfile
                    {
                        RiskLevel = "Vysoké",
                        Description = "Cílíte na maximální výnosy a akceptujete vysoké riziko.",
                        AssetAllocation = new Dictionary<string, double>
                        {
                            { "Hotovost a krátkodobé instrumenty", 0.05 },
                            { "Dluhopisy", 0.10 },
                            { "Akcie", 0.65 },
                            { "Alternativní investice", 0.20 }
                        }
                    }
                }
            };
        }

        public List<RiskProfileQuestion> GetQuestions()
        {
            return _questions;
        }

        public InvestorProfile EvaluateAnswers(List<int> answers)
        {
            if (answers.Count != _questions.Count)
                throw new ArgumentException("Počet odpovědí neodpovídá počtu otázek");

            int totalScore = answers.Sum();
            int maxScore = _questions.Count * 4;

            // Výpočet percentilu
            double percentage = (double)totalScore / maxScore;

            // Určení profilu na základě percentilu
            if (percentage <= 0.25)
                return _profiles["Konzervativní"];
            else if (percentage <= 0.5)
                return _profiles["Vyvážený"];
            else if (percentage <= 0.75)
                return _profiles["Růstový"];
            else
                return _profiles["Dynamický"];
        }
    }

    public class InvestmentStrategy
    {
        private readonly InvestorProfile _profile;

        public InvestmentStrategy(InvestorProfile profile)
        {
            _profile = profile;
        }

        public string GenerateRecommendations()
        {
            var recommendations = new List<string>
            {
                $"Váš investiční profil: {_profile.RiskLevel}",
                $"Charakteristika: {_profile.Description}",
                "\nDoporučená alokace aktiv:"
            };

            foreach (var allocation in _profile.AssetAllocation)
            {
                recommendations.Add($"- {allocation.Key}: {allocation.Value:P0}");
            }

            recommendations.Add("\nKonkrétní doporučení:");

            switch (_profile.RiskLevel)
            {
                case "Nízké":
                    recommendations.AddRange(new[]
                    {
                        "- Zaměřte se na státní dluhopisy a termínované vklady",
                        "- Zvažte konzervativní dluhopisové fondy",
                        "- Udržujte vyšší podíl hotovosti pro příležitosti",
                        "- Diverzifikujte mezi více bank a emitentů"
                    });
                    break;

                case "Střední":
                    recommendations.AddRange(new[]
                    {
                        "- Kombinujte dluhopisové a akciové fondy",
                        "- Zvažte ETF kopírující hlavní akciové indexy",
                        "- Přidejte korporátní dluhopisy pro vyšší výnos",
                        "- Udržujte vyváženou geografickou diverzifikaci"
                    });
                    break;

                case "Vyšší":
                    recommendations.AddRange(new[]
                    {
                        "- Zaměřte se na růstové akcie a sektory",
                        "- Zvažte přímé investice do akcií",
                        "- Přidejte realitní fondy nebo REIT",
                        "- Můžete zkusit menší pozice v kryptoměnách"
                    });
                    break;

                case "Vysoké":
                    recommendations.AddRange(new[]
                    {
                        "- Využívejte aktivní trading strategie",
                        "- Zvažte pákové produkty a deriváty",
                        "- Investujte do vznikajících trhů a technologií",
                        "- Diverzifikujte pomocí kryptoměn a komodit"
                    });
                    break;
            }

            return string.Join("\n", recommendations);
        }
    }
}