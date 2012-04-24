﻿using System;
using System.Text.RegularExpressions;

namespace IniParser.Parser.Configurations
{
    /// <summary>
    ///     Configuration data used for a <see cref="IniDataParser"/> class instance.
    /// </summary>
    /// 
    /// This class allows changing the behaviour of a <see cref="IniDataParser"/> instance. The <see cref="IniDataParser"/>
    /// exposes an instance of this class via property <see cref="IniDataParser.Configuration"/>
    public abstract class BaseParserConfiguration : IParserConfiguration
    {
        /// <summary>
        ///     Regular expression for matching a comment string
        /// </summary>
        public Regex CommentRegex { get; set; }

        /// <summary>
        ///     Regular expression for matching a section string
        /// </summary>
        public Regex SectionRegex { get; set; }

        /// <summary>
        ///     Regular expression for matching a key / value pair string
        /// </summary>
        public Regex KeyValuePairRegex { get; set; }


        /// <summary>
        ///     Sets the char that defines the start of a section name.
        /// </summary>
        /// <remarks>
        ///     Defaults to character '['
        /// </remarks>
        public char SectionStartChar
        {
            get { return _sectionStartChar; }
            set
            {
                _sectionStartChar = value;
                RecreateSectionRegex(_sectionStartChar);
            }
        }

        /// <summary>
        ///     Sets the char that defines the end of a section name.
        /// </summary>
        /// <remarks>
        ///     Defaults to character ']'
        /// </remarks>
        public char SectionEndChar
        {
            get { return _sectionEndChar; }
            set
            {
                _sectionEndChar = value;
                RecreateSectionRegex(_sectionEndChar);
            }
        }

        /// <summary>
        ///     Sets the char that defines the start of a comment.
        ///     A comment spans from the comment character to the end of the line.
        /// </summary>
        /// <remarks>
        ///     Defaults to character '#'
        /// </remarks>
        public char CommentChar
        {
            get { return _commentChar; }
            set 
            { 
                CommentRegex = new Regex(value + _strCommentRegex);
                _commentChar = value;
            }
        }

        /// <summary>
        ///     Sets the char that defines a value assigned to a key
        /// </summary>
        /// <remarks>
        ///     Defaults to character '='
        /// </remarks>
        public char KeyValueAssigmentChar
        {
            get { return _keyValueAssigmentChar; }
            set
            {
                KeyValuePairRegex = new Regex(_strKeyRegex + value + _strValueRegex);
                
                _keyValueAssigmentChar = value;
            }
        }

        /// <summary>
        ///     Allows having keys in the file that don't belong to any section.
        ///     i.e. allows defining keys before defining a section.
        ///     If set to <c>false</c> and keys without a section are defined, 
        ///     the <see cref="IniDataParser"/> will stop with an error.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>true</c>.
        /// </remarks>
        public bool AllowKeysWithoutSection { get; set; }

        /// <summary>
        ///     If set to <c>false</c> and the <see cref="IniDataParser"/> finds duplicate keys in a
        ///     section the parser will stop with an error.
        ///     If set to <c>true</c>, duplicated keys are allowed in the file. The value
        ///     of the duplicate key will be the last value asigned to the key in the file.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        public bool AllowDuplicateKeys { get; set; }

        /// <summary>
        ///     If <c>true</c> the <see cref="IniDataParser"/> instance will thrown an exception
        ///     if an error is found. 
        ///     If <c>false</c> the parser will just stop execution and return a null value.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>true</c>.
        /// </remarks>
        public bool ThrowExceptionsOnError { get; set; }

        /// <summary>
        ///     If set to <c>false</c> and the <see cref="IniDataParser"/> finds a duplicate section
        ///     the parser will stop with an error.
        ///     If set to <c>true</c>, duplicated sections are allowed in the file, but only a 
        ///     <see cref="SectionData"/> element will be created in the <see cref="IniData.Sections"/>
        ///     collection.
        /// </summary>
        /// <remarks>
        ///     Defaults to <c>false</c>.
        /// </remarks>
        public bool AllowDuplicateSections { get; set; }

        #region Fields
        

        private char _keyValueAssigmentChar;
        private char _sectionStartChar;
        private char _sectionEndChar;
        private char _commentChar;

        #endregion

        #region Constants
        protected const string _strCommentRegex = @".*";
        protected const string _strSectionRegexStart = @"^(\s*?)";
        protected const string _strSectionRegexMiddle = @"{1}\s*[_\{\}\#\+\;\%\(\)\=\?\&\$\,\:\/\.\-\w\d\s]+\s*"; //@"{1}\s*[_\#\+\;\%\(\)\=\?\&\$\,\:\/\.\-\w\d\s]+\s*";
        protected const string _strSectionRegexEnd = @"(\s*?)$";
        protected const string _strKeyRegex = @"^(\s*[_\.\d\w]*\s*)";
        protected const string _strValueRegex = @"([\s\d\w\W\.]*)$";
        protected const string _strSpecialRegexChars = @"[\^$.|?*+()";
        #endregion
        
        #region Helpers
        private void RecreateSectionRegex(char value)
        {
            if (char.IsControl(value)
                || char.IsWhiteSpace(value)
                || value == CommentChar
                || value == KeyValueAssigmentChar)
                throw new Exception(string.Format("Invalid character for section delimiter: '{0}",
                                                              value));

            string builtRegexString = _strSectionRegexStart;

            if (_strSpecialRegexChars.Contains(new string(_sectionStartChar, 1)))
                builtRegexString += "\\" + _sectionStartChar;
            else builtRegexString += _sectionStartChar;

            builtRegexString += _strSectionRegexMiddle;

            if (_strSpecialRegexChars.Contains(new string(_sectionEndChar, 1)))
                builtRegexString += "\\" + _sectionEndChar;
            else
                builtRegexString += _sectionEndChar;

            builtRegexString += _strSectionRegexEnd;

            SectionRegex = new Regex(builtRegexString);
        }
        #endregion
    }
}