﻿using System;
using Popcorn.Models.Localization;
using Popcorn.Models.Subtitle.Json;
using Popcorn.Helpers;

namespace Popcorn.Models.Subtitle
{
    public class Subtitle : SubtitleDeserialized, IComparable<Subtitle>
    {
        #region Properties

        #region Property -> Language

        private ILanguage _language;

        /// <summary>
        /// Language's subtitle
        /// </summary>
        public ILanguage Language
        {
            get { return _language; }
            set
            {
                Set(() => Language, ref _language, value);
                SetFlagImagePath();
            }
        }

        #endregion

        #region Property -> FlagImagePath

        private string _flagImagePath;

        /// <summary>
        /// Flag image's path
        /// </summary>
        public string FlagImagePath
        {
            get { return _flagImagePath; }
            set { Set(() => FlagImagePath, ref _flagImagePath, value); }
        }

        #endregion

        #region Property -> FilePath

        private string _filePath;

        /// <summary>
        /// Subtitle file path
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { Set(() => FilePath, ref _filePath, value); }
        }

        #endregion

        #endregion

        #region Method -> SetFlagImagePath

        /// <summary>
        /// Set file path of flag image depending on its language
        /// </summary>
        private void SetFlagImagePath()
        {
            switch (Language.EnglishName)
            {
                case "english":
                    FlagImagePath = Constants.FlagImagesDirectory + "gb.png";
                    break;
                case "brazilian-portuguese":
                    FlagImagePath = Constants.FlagImagesDirectory + "br.png";
                    break;
                case "danish":
                    FlagImagePath = Constants.FlagImagesDirectory + "dk.png";
                    break;
                case "dutch":
                    FlagImagePath = Constants.FlagImagesDirectory + "be.png";
                    break;
                case "german":
                    FlagImagePath = Constants.FlagImagesDirectory + "de.png";
                    break;
                case "japanese":
                    FlagImagePath = Constants.FlagImagesDirectory + "jp.png";
                    break;
                case "swedish":
                    FlagImagePath = Constants.FlagImagesDirectory + "fi.png";
                    break;
                case "polish":
                    FlagImagePath = Constants.FlagImagesDirectory + "pl.png";
                    break;
                case "bulgarian":
                    FlagImagePath = Constants.FlagImagesDirectory + "bg.png";
                    break;
                case "farsi-persian":
                    FlagImagePath = Constants.FlagImagesDirectory + "ir.png";
                    break;
                case "finnish":
                    FlagImagePath = Constants.FlagImagesDirectory + "fi.png";
                    break;
                case "greek":
                    FlagImagePath = Constants.FlagImagesDirectory + "gr.png";
                    break;
                case "indonesian":
                    FlagImagePath = Constants.FlagImagesDirectory + "id.png";
                    break;
                case "korean":
                    FlagImagePath = Constants.FlagImagesDirectory + "kr.png";
                    break;
                case "malay":
                    FlagImagePath = Constants.FlagImagesDirectory + "bn.png";
                    break;
                case "portuguese":
                    FlagImagePath = Constants.FlagImagesDirectory + "br.png";
                    break;
                case "spanish":
                    FlagImagePath = Constants.FlagImagesDirectory + "es.png";
                    break;
                case "turkish":
                    FlagImagePath = Constants.FlagImagesDirectory + "tr.png";
                    break;
                case "vietnamese":
                    FlagImagePath = Constants.FlagImagesDirectory + "vn.png";
                    break;
                case "french":
                    FlagImagePath = Constants.FlagImagesDirectory + "fr.png";
                    break;
                case "serbian":
                    FlagImagePath = Constants.FlagImagesDirectory + "rs.png";
                    break;
                case "arabic":
                    FlagImagePath = Constants.FlagImagesDirectory + "dz.png";
                    break;
                case "romanian":
                    FlagImagePath = Constants.FlagImagesDirectory + "ro.png";
                    break;
                case "croatian":
                    FlagImagePath = Constants.FlagImagesDirectory + "hr.png";
                    break;
                case "hebrew":
                    FlagImagePath = Constants.FlagImagesDirectory + "il.png";
                    break;
                case "norwegian":
                    FlagImagePath = Constants.FlagImagesDirectory + "no.png";
                    break;
                case "italian":
                    FlagImagePath = Constants.FlagImagesDirectory + "it.png";
                    break;
                case "russian":
                    FlagImagePath = Constants.FlagImagesDirectory + "ru.png";
                    break;
                case "chinese":
                    FlagImagePath = Constants.FlagImagesDirectory + "cn.png";
                    break;
                case "czech":
                    FlagImagePath = Constants.FlagImagesDirectory + "cz.png";
                    break;
                case "slovenian":
                    FlagImagePath = Constants.FlagImagesDirectory + "si.png";
                    break;
                case "hungarian":
                    FlagImagePath = Constants.FlagImagesDirectory + "hu.png";
                    break;
                case "bengali":
                    FlagImagePath = Constants.FlagImagesDirectory + "in.png";
                    break;
            }

            #endregion
        }

        /// <summary>
        /// Used to comapre subtitle by english name
        /// </summary>
        /// <param name="subtitle">The subtitle to compare with</param>
        /// <returns></returns>
        public int CompareTo(Subtitle subtitle)
        {
            return string.Compare(Language.EnglishName, subtitle.Language.EnglishName, StringComparison.CurrentCulture);
        }
    }
}