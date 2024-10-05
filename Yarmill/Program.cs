
class Program
{
    public static void Main(string[] args)
    {

    }
    static void NacteniDat()
    {
        HttpClient client = new HttpClient();
        string BaseUrl = "https://www.trampolinescore.com";

        // Stáhni hlavní stránku s odkazy
        string url = "https://www.trampolinescore.com/events";
        HttpResponseMessage response = await client.GetAsync(url);
        string pageContent = await response.Content.ReadAsStringAsync();

        // Načti HTML dokument
        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(pageContent);

        // Najdi všechny odkazy uvnitř <div> s třídou exev__item exevit
        var eventLinks = document.DocumentNode.SelectNodes("//div[@class="exev__item exevit"]");

        //Seznam pro uložení závodníků
        List<EventData> eventDataList = new List<EventData>();

        // Projdi všechny odkazy a přidej base URL
        foreach (var eventDiv in eventLinks)
        {
            // Získání hodnoty atributu data-click-href
            string hrefValue = eventDiv.GetAttributeValue("data-click-href", string.Empty);

            if (!string.IsNullOrEmpty(hrefValue))
            {
                // Přidej základní URL ke každému odkazu
                string fullUrl = $"{baseUrl}{hrefValue}";

                // Načti stránku pro každý event
                HttpResponseMessage eventResponse = await client.GetAsync(fullUrl);
                string eventPageContent = await eventResponse.Content.ReadAsStringAsync();

                // Načti HTML dokument pro stránku eventu
                HtmlDocument eventDocument = new HtmlDocument();
                eventDocument.LoadHtml(eventPageContent);

                // Extrahuj data o závodníkovi z tabulky (předpokládejme, že tabulka má specifickou strukturu)
                var rows = eventDocument.DocumentNode.SelectNodes("//table[@class='some-class-name']//tr"); //Zmeniiit class!!!!!!!!!!

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes("td");

                        if (cells != null && cells.Count >= 10)
                        {
                            // Ověření, zda je jméno závodníka shodné se zadaným jménem
                            string name = cells[0].InnerText.Trim();
                            if (name.Equals(searchName, StringComparison.OrdinalIgnoreCase))
                            {
                                EventData eventData = new EventData
                                {
                                    Name = name,
                                    Date = cells[1].InnerText.Trim(), // Datum závodu
                                    Location = cells[2].InnerText.Trim(), // Místo konání
                                    Stage = cells[3].InnerText.Trim(), // Kvalifikace nebo finále
                                    Type = cells[4].InnerText.Trim(), // Synchrony nebo jednotlivci
                                    Competition = cells[5].InnerText.Trim(), // Typ závodu
                                    Position = cells[6].InnerText.Trim(), // Umístění
                                    ToF = cells[7].InnerText.Trim(), // ToF
                                    Diff = cells[8].InnerText.Trim(), // Obtížnost
                                    HD = cells[9].InnerText.Trim(), // HD
                                    Execution = cells[10].InnerText.Trim(), // Provedení
                                    RoutinePoints = cells[11].InnerText.Trim(), // Body za sestavu
                                    SetType = cells[12].InnerText.Trim(), // Povinná nebo volná sestava
                                    EventUrl = fullUrl // URL události
                                };

                                eventDataList.Add(eventData);
                            }
                        }
                    }
                }
            }
        }
        // Vypiš uložená data o závodníkovi
        if (eventDataList.Count > 0)
        {
            foreach (var data in eventDataList)
            {
                Console.WriteLine(data);
            }
        }
        else
        {
            Console.WriteLine("Závodník nenalezen.");
        }

        // Po ukončení práce s klientem je vhodné jej uvolnit
        client.Dispose();
    }
}