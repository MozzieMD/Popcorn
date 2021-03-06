﻿using GalaSoft.MvvmLight;

namespace Popcorn.Models.Localization
{
    /// <summary>
    /// French language
    /// </summary>
    public sealed class FrenchLanguage : ObservableObject, ILanguage
    {
        #region Properties

        #region Property -> LocalizedName

        /// <summary>
        /// Language's name
        /// </summary>
        public string LocalizedName { get; }

        #endregion

        #region Property -> EnglishName

        /// <summary>
        /// English language's name
        /// </summary>
        public string EnglishName { get; }

        #endregion

        #region Property -> Culture

        /// <summary>
        /// Language's culture
        /// </summary>
        public string Culture { get; set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FrenchLanguage()
        {
            LocalizedName = "Français";
            EnglishName = "French";
            Culture = "fr";
        }

        #endregion

        #region Methods

        #region Method -> Equals

        /// <summary>
        /// Check equality based on is localized name
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            var item = obj as FrenchLanguage;

            if (item == null)
            {
                return false;
            }

            return LocalizedName.Equals(item.LocalizedName);
        }

        #endregion

        #region Method -> GetHashCode

        /// <summary>
        /// Get hash code based on it localized name
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return LocalizedName.GetHashCode();
        }

        #endregion

        #endregion
    }
}