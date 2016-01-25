
namespace Srk.BetaseriesApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Srk.BetaseriesApi.Resources;

    /// <summary>
    /// To work with badges :)
    /// </summary>
    public static class BadgesUtil
    {
        static BadgesUtil()
        {
            serviceMap.Add("amateur", Badges.Amateur);
            serviceMap.Add("blogueur_influent", Badges.BlogueurInfluent);
            serviceMap.Add("capitaine_spock", Badges.CapitaineSpock);
            serviceMap.Add("confirme", Badges.Confirme);
            serviceMap.Add("debutant", Badges.Debutant);
            serviceMap.Add("drama_queen", Badges.DramaQueen);
            serviceMap.Add("developpeur", Badges.Developpeur);
            serviceMap.Add("ecole_buissoniere", Badges.EcoleBuissoniere);
            serviceMap.Add("einstein", Badges.Einstein);
            serviceMap.Add("flashmob", Badges.Flashmob);
            serviceMap.Add("gendarme", Badges.Gendarme);
            serviceMap.Add("glandeur", Badges.Glandeur);
            serviceMap.Add("hadopi_master", Badges.HadopiMaster);
            serviceMap.Add("hyperactifs", Badges.Hyperactifs);
            serviceMap.Add("jerry_seinfeld", Badges.JerrySeinfeld);
            serviceMap.Add("joyeux_noel", Badges.JoyeuxNoel);
            serviceMap.Add("junior", Badges.Junior);
            serviceMap.Add("le_millier", Badges.LeMillier);
            serviceMap.Add("marathonien", Badges.Marathonien);
            serviceMap.Add("martine_aubry", Badges.MartineAubry);
            serviceMap.Add("meilleur_ami", Badges.MeilleurAmi);
            serviceMap.Add("noctambule", Badges.Noctambule);
            serviceMap.Add("old_school", Badges.OldSchool);
            serviceMap.Add("parrain", Badges.Parrain);
            serviceMap.Add("petit_bavard", Badges.PetitBavard);
            serviceMap.Add("planque", Badges.Planque);
            serviceMap.Add("poke_her_face", Badges.PokeHerFace);
            serviceMap.Add("rattrapage", Badges.Rattrapage);
            serviceMap.Add("senior", Badges.Senior);
            serviceMap.Add("serievore", Badges.Serievore);
            serviceMap.Add("superstar", Badges.Superstar);
            serviceMap.Add("surbooking", Badges.Surbooking);
            serviceMap.Add("tas_dbeaux_yeux", Badges.BeauxYeux);
            serviceMap.Add("twittos", Badges.Twittos);
            serviceMap.Add("vieux_briscard", Badges.VieuxBriscard);
            serviceMap.Add("lunettes_3D", Badges.Lunettes3d);
            // All badges below are unsure
            serviceMap.Add("bienfaiteur", Badges.Bienfaiteur);
            serviceMap.Add("donateur", Badges.Donateur);
            serviceMap.Add("fiche", Badges.Fiche);
            serviceMap.Add("forumeur_debutant", Badges.ForumStarter);
            serviceMap.Add("peinture_fraiche", Badges.FreshPaint);
            // added on 2016-01-20
            serviceMap.Add("apple_fanboy", Badges.AppleFanboy);
            serviceMap.Add("cineaste", Badges.Cineaste);
            serviceMap.Add("courrier_du_coeur", Badges.CourrierDuCoeur);
            serviceMap.Add("forumeur_confirme", Badges.ForumeurConfirme);
            serviceMap.Add("forumeur_expert", Badges.ForumeurExpert);
            serviceMap.Add("lapinou", Badges.Lapinou);
        }

        private static readonly Dictionary<string, Badges> serviceMap = new Dictionary<string, Badges>();

        /// <summary>
        /// Return known service keys.
        /// </summary>
        public static string[] ServiceKeys
        {
            get { return serviceMap.Keys.ToArray(); }
        }

        /// <summary>
        /// Try to parse a badge from a service key.
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="badge"></param>
        /// <returns>true if parsing succeed</returns>
        public static bool TryParseBadge(string serviceKey, out Badges badge)
        {
            if (serviceMap.ContainsKey(serviceKey))
            {
                badge = serviceMap[serviceKey];
                return true;
            }
            else
            {
                badge = Badges.Unknown;
                return false;
            }
        }

        /// <summary>
        /// Returns a translated name for a badge.
        /// If no translation is available, the input value is returned.
        /// </summary>
        /// <param name="badge"></param>
        /// <returns></returns>
        public static string GetName(Badges badge)
        {
            return GetName(badge.ToString());
        }

        /// <summary>
        /// Returns a translated name for a badge.
        /// If no translation is available, the input value is returned.
        /// </summary>
        /// <param name="internalKey">key is Badges.ToString()</param>
        /// <returns></returns>
        public static string GetName(string internalKey)
        {
            try
            {
                return GeneralStrings.ResourceManager.GetString(
                    string.Concat("Badges_", internalKey)
                ) ?? internalKey;
            }
            catch
            {
                return internalKey;
            }
        }

        /// <summary>
        /// Returns a translated name for a badge.
        /// If no translation is available, null is returned.
        /// </summary>
        /// <param name="badge"></param>
        /// <returns></returns>
        public static string GetTranslatedName(Badges badge)
        {
            return GetTranslatedName(badge.ToString());
        }

        /// <summary>
        /// Returns a translated name for a badge.
        /// If no translation is available, null is returned.
        /// </summary>
        /// <param name="internalKey">key is Badges.ToString()</param>
        /// <returns></returns>
        public static string GetTranslatedName(string internalKey)
        {
            try
            {
                return GeneralStrings.ResourceManager.GetString(
                    string.Concat("Badges_", internalKey)
                );
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a translated description for a badge.
        /// </summary>
        /// <param name="badge"></param>
        /// <returns></returns>
        public static string GetDescription(Badges badge)
        {
            return GetDescription(badge.ToString());
        }

        /// <summary>
        /// Returns a translated description for a badge.
        /// </summary>
        /// <param name="internalKey">key is Badges.ToString()</param>
        /// <returns></returns>
        public static string GetDescription(string internalKey)
        {
            try
            {
                return GeneralStrings.ResourceManager.GetString(
                    string.Concat("BadgeDesc_", internalKey)
                );
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Return the appropriate enum value from a badge key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Badges Parse(string key)
        {
            return serviceMap.ContainsKey(key) ? serviceMap[key] : Badges.Unknown;
        }

    }

    /// <summary>
    /// Enumeration of betaseries badges.
    /// </summary>
    public enum Badges
    {
        /// <summary>
        /// Badly parsed badges or new one.
        /// </summary>
        Unknown,

        /// <summary>
        /// Ces douze dernières heures vous avez regardé 4 épisodes. Faites quelque chose de votre vie.
        /// </summary>
        Planque,

        /// <summary>
        /// Quand on passe sa soirée à regarder 4 épisodes, faut pas s'étonner de ce qu'on dit.
        /// </summary>
        Glandeur,

        /// <summary>
        /// Vous avez regardé plus de 35 heures d'épisodes. C'est la lutte...
        /// </summary>
        MartineAubry,

        /// <summary>
        /// Vous avez regardé le même épisode que 75 personnes ce jour-là, vous êtes à la mode.
        /// </summary>
        Flashmob,

        /// <summary>
        /// Une semaine, dix épisodes. Il va falloir relativiser le temps, et votre vie.
        /// </summary>
        Einstein,

        /// <summary>
        /// 3 épisodes d'une même série à la suite... On a une série à rattraper, à ce que je vois.
        /// </summary>
        Rattrapage,

        /// <summary>
        /// 50 personnes vous ont ajouté à leurs amis, Michel Drucker n'a qu'à bien se tenir.
        /// </summary>
        Superstar,

        /// <summary>
        /// Vous avez regardé 6 épisodes en plein dimanche, c'est un grand chelem !
        /// </summary>
        Marathonien,

        /// <summary>
        /// Ça y est, vous avez vu plus de 1000 épisodes de séries télé. Sale geek.
        /// </summary>
        LeMillier,

        /// <summary>
        /// Vous vous êtes inscrit pendant la première année d'existence de BetaSeries.
        /// </summary>
        OldSchool,

        /// <summary>
        /// Vous avez parrainé un ami sur BetaSeries, maintenant il faut s'en occuper.
        /// </summary>
        Parrain,

        /// <summary>
        /// Grâce à vous, déjà 10 sous-titres ont été signalés comme inadéquats.
        /// </summary>
        Gendarme,

        /// <summary>
        /// Vous avez posté 25 commentaires sur BetaSeries, que de bla bla !
        /// </summary>
        PetitBavard,

        /// <summary>
        /// Woh, 12 badges sur votre profil, on va plus avoir la place pour d'autres.
        /// </summary>
        Surbooking,

        /// <summary>
        /// Bonne lecture avec vos 100 fichiers de sous-titres téléchargés...
        /// </summary>
        HadopiMaster,

        /// <summary>
        /// 10 séries humoristiques, on a un peu peur pour vous, respirez de temps en temps.
        /// </summary>
        JerrySeinfeld,

        /// <summary>
        /// 50 séries, on ne peut plus vous arrêter. Continuez, après tout, ça ne nous dérange pas.
        /// </summary>
        Senior,

        /// <summary>
        /// Vous avez activé Facebook sur votre compte BetaSeries, vous êtes social.
        /// </summary>
        PokeHerFace,

        /// <summary>
        /// Votre compte Twitter est configuré, vous faites partie de la grande famille du microblogging.
        /// </summary>
        Twittos,

        /// <summary>
        /// Votre dixième série ajoutée, vous commencez à avoir un profil intéressant.
        /// </summary>
        Amateur,

        /// <summary>
        /// 25 séries regardées, vous commencez à devenir accro, faites attention !
        /// </summary>
        Confirme,

        /// <summary>
        /// Un mois s'est écoulé et 30 épisodes ont été regardés, vous avez le double-clic facile.
        /// </summary>
        Serievore,

        /// <summary>
        /// Votre magnifique trombine est maintenant sur votre profil, c'est pour la drague ?
        /// </summary>
        BeauxYeux,

        /// <summary>
        /// Vous avez regardé votre premier épisode. Bienvenue sur BetaSeries !
        /// </summary>
        Debutant,

        /// <summary>
        /// Vous osez regarder encore des séries après 3 heures du matin, en pleine semaine...
        /// </summary>
        EcoleBuissoniere,

        /// <summary>
        /// Vous suivez 5 séries fantastiques, attention aux extra-terrestres.
        /// </summary>
        CapitaineSpock,

        /// <summary>
        /// 10 amis réciproques. C'est beau l'amitié, surtout quand l'autre est au courant.
        /// </summary>
        MeilleurAmi,

        /// <summary>
        /// Vos amis ont effectué 30 actions dans les dernières 24 heures, ce sont des clients fidèles.
        /// </summary>
        Hyperactifs,

        /// <summary>
        /// Cela fait un an que vous êtes inscrit sur BetaSeries, ça se fête.
        /// </summary>
        VieuxBriscard,

        /// <summary>
        /// Vous avez regardé 10 épisodes de 10 séries différentes, bon début.
        /// </summary>
        Junior,

        /// <summary>
        /// 10 drama dans votre compte ? Faudra prévoir les mouchoirs la prochaine fois.
        /// </summary>
        DramaQueen,

        /// <summary>
        /// Vous avez suggéré 25 fois à vos amis de suivre une série, vous ont-ils écouté ?
        /// </summary>
        BlogueurInfluent,

        /// <summary>
        /// Vous utilisez vos petites mains pleines de doigts pour améliorer l'écosystème de BetaSeries. Merci mille fois !
        /// </summary>
        Developpeur,

        /// <summary>
        /// Vous avez préféré regarder un épisode plutôt que de vous taper les repas de famille. On vous comprend.
        /// </summary>
        JoyeuxNoel,

        /// <summary>
        /// Sept nuits, sept épisodes. On a ses petites habitudes, à ce que je vois.
        /// </summary>
        Noctambule,

        FreshPaint,
        Bienfaiteur,
        Donateur,
        Fiche,
        ForumStarter,

        /// <summary>
        /// Cent films dans votre liste, il va falloir penser à aller voir une séance 3D. Comment ça, non ?
        /// </summary>
        Lunettes3d,
        AppleFanboy,
        Cineaste,
        CourrierDuCoeur,
        ForumeurConfirme,
        ForumeurExpert,
        Lapinou,
    }
}
