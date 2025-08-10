using Ark.Data;

namespace Ark.Net.MqSeries.Ftp
{
    /// <summary>
    /// Cette entité contient les informations pour aller télécharger un fichier sur le mainframe
    /// </summary>
    [MainFrameObject]
    public partial class MainFrameFileMfEntity
    {
        /// <summary>
        /// Nom de l'application
        /// </summary>
        [MainFrameProperty(Length = 10)]
        public string NomApplication { get; set; }

        /// <summary>
        /// Chemin d'accès sur le Ftp du mainframe au fichier à télécharger
        /// </summary>
        [MainFrameProperty(Length = 140)]
        public string CheminFtp { get; set; }

        /// <summary>
        /// Chemin où il faut déposer le fichier.
        /// Pas toujours nécéssaire.
        /// </summary>
        [MainFrameProperty(Length = 140)]
        public string CheminDestination { get; set; }

        /// <summary>
        /// Nom de la file Mq serie
        /// Pas toujours nécessaire.
        /// </summary>
        [MainFrameProperty(Length = 48)]
        public string FileMq { get; set; }
    }
}
