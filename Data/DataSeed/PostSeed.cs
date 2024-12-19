using Domain;

namespace DAL.DataSeed
{
    public class PostSeed
    {
        public static async Task Seed(BlogDbContext dbContext)
        {
            if (!dbContext.Posts.Any())
            {
                var bos = new Post()
                {
                    User = dbContext.Users.First(e => e.Username == "1937nkvd"),
                    Title = "Brotherhood Of Steel",
                    Content =
                        "The Brotherhood of Steel (commonly abbreviated to BoS) is a post-War techno-religious military order with chapters operating across the territory of the former United States.[Non-game 1] Founded by rogue U.S. Army Captain Roger Maxson shortly after the Great War, the Brotherhood's core purpose is to preserve advanced technology and regulate its usage.[4][5] Though small and relatively isolationist, the Brotherhood has proved to be one of the most important organizations in the history of the wasteland, though their exact levels of power and influence have varied over time and by chapter.The Brotherhood has been featured in every game in the Fallout series, in one form or another.This article focuses exclusively on an overview of the Brotherhood as it appears throughout the series.For information on specific Brotherhood chapters,                    see: Brotherhood of Steel chapters"
                };

                var ncr = new Post()
                {
                    User = dbContext.Users.First(e => e.Username == "1937nkvd"),
                    Title = "New California Republic",
                    Content =
                        "The New California Republic (NCR) is a post-War federal republic founded in New California in 2189. It is comprised of five contiguous states located in southern California,[4][5] with additional territorial holdings in northern California,[6][7] Oregon[8] and Nevada.[9][10]  Considering itself an inheritor to pre-war values of democracy, personal liberty and rule of law, the NCR has developed a highly expansionist culture, undertaking colonization efforts and military expeditions into neighboring regions in order to \"civilize\" them.[11] However these policies are not without controversy, and by 2281 many within and outside the Republic have come to criticize it as hawkish and prone to corruption.  The New California Republic is mentioned in the endings for Fallout, and grows to become a major faction in two subsequent games focusing on geopolitical struggles: In Fallout 2 it faces New Reno and Vault City in the competition for control over the northern half of California, while in Fallout: New Vegas it faces the Caesar's Legion in a titanic struggle for control over the Mojave and the future of the American Southwest which, according to some, can be described as the single greatest geopolitical conflict since the Great War.[12][13]"
                };

                var legion = new Post()
                {
                    User = dbContext.Users.First(e => e.Username == "1937nkvd"),
                    Title = "Caesars Legion",
                    Content =
                        "Caesar's Legion (Latin: Legio Caesaris),[1] also referred to simply as The Legion, is a totalitarian dictatorship founded in 2247 by Edward \"Caesar\" Sallow and Joshua Graham, built on the conquest and enslavement of tribal societies in the American southwest. To enforce unity in the absence of any civilian institutions, the Legion loosely models itself after the military of the Roman Empire, repurposing its language and aesthetics for the post-apocalypse.[2][3]  As of 2281, the Legion controls large amounts of territory east of the Colorado River, primarily in the former states of Arizona and New Mexico, with footholds in Utah and Colorado.[4] Worshipped as the \"son of Mars\" by his followers, Caesar's ultimate goal is to conquer the New California Republic and merge its civil institutions and infrastructure with the military strength of the Legion, creating a new totalitarian empire.[5"
                };

                var klamath = new Post()
                {
                    User = dbContext.Users.First(e => e.Username == "1937nkvd"),
                    Title = "Klamath",
                    Content =
                        "It is a small community of trappers that hunt the giant, mutant lizards called geckos in the area. Highly prized for their pelts, especially those of the golden geckos, they are the lifeblood of this community. Klamath is the stopping point for caravans on their way to the tiny tribal villages to the north, like Arroyo, and also serves as a place where members of those tribes can come and exchange information, goods, and news about the larger world.  A few months prior to the Chosen One coming to Klamath, the locals start to believe the mechanical screeching of the Mister Handy to be a spirit.[2][3]  An Enclave Vertibird piloted by Daisy Whitman, crashed in the nearby canyon due to a rotor malfunction.[4] A prop blade of which eventually was scavenged by Lily Bowen to make her sword.[5]  The city was plagued by rats. Vic the trader, who lives in Klamath, has been captured by Metzger and needs to be rescued. The Chosen One visited Klamath and made the town a little more exciting for some time. Forty years on, it is still the same town where one can hunt, eat and skin geckos for pelts all day long, according to Klamath Bob.[6]"
                };

                var boone = new Post()
                {
                    User = dbContext.Users.First(e => e.Username == "1937nkvd"),
                    Title = "Boone",
                    Content =
                        "Craig Boone is a retired NCR First Recon sharpshooter and Novac's night-shift town guard in 2281. He is a possible companion.  Craig Boone joined the New California Republic Army and served until some time after the Bitter Springs Massacre. He completed the majority of his army service with the First Recon, moved to Novac with his wife Carla Boone, and became the town's night watchman. However, Carla was recently kidnapped by Caesar's Legion while he was on guard and he suspects foul play. When the Courier first meets him, Boone is bitterly whiling away the hours until he can find out and kill whoever is responsible for the disappearance of his wife; waiting for the perfect stranger to arrive and to hopefully help find who facilitated the taking of his wife.[1] Hardened and psychologically troubled from his time in the NCR military, Boone vacillates between being a stone-cold killer and a decent human being. Venturing out from the relative safety of Novac brings Boone into close contact with his old life until he is forced to deal with the tragic events that caused him to leave the military."
                };

                var mrNewVegas = new Post()
                {
                    User = dbContext.Users.First(e => e.Username == "1937nkvd"),
                    Title = "Mr. New Vegas",
                    Content =
                        "Mr. New Vegas is an AI personality which was created by Mr. House[1] before the Great War to be the DJ of Radio New Vegas,[2] a job he still performs over 200 years later in 2281. He is heard on Radio New Vegas, but never seen in-game."
                };

                var yesMan = new Post()
                {
                    User = dbContext.Users.First(e => e.Username == "1937nkvd"),
                    Title = "Yes Man",
                    Content =
                        "Yes Man is an AI program that serves as Benny's assistant and right-hand man in 2281. He is a modified PDQ-88b securitron with a unique personality, serving as an integral part of Benny's scheme to take control of New Vegas. Yes Man will assist the Courier directly in the Independent New Vegas main questline."
                };

                dbContext.Add(yesMan);
                dbContext.Add(mrNewVegas);
                dbContext.Add(boone);
                dbContext.Add(klamath);
                dbContext.Add(ncr);
                dbContext.Add(legion);
                dbContext.Add(bos);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}