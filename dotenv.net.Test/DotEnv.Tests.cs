﻿using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace dotenv.net.Test
{
    public class DotEnvTests
    {
        private const string WhitespacesEnvFileName = "values-with-whitespaces.env";
        private const string ValuesAndCommentsEnvFileName = "values-and-comments.env";
        private const string NonExistentEnvFileName = "non-existent.env";

        [Fact]
        public void ThrowsExceptionWithNonExistentEnvFileWhenThrowErrorIsTrue()
        {
            Action action = () => DotEnv.Config(true, NonExistentEnvFileName);
            action.ShouldThrowExactly<FileNotFoundException>()
                .WithMessage($"An environment file with path \"{NonExistentEnvFileName}\" does not exist.");
        }

        [Fact]
        public void DoesNotThrowExceptionWithNonExistentEnvFileWhenThrowErrorIsFalse()
        {
            Action action = () => DotEnv.Config(false, "non-existent.env");
            action.ShouldNotThrow();
        }

        [Fact]
        public void AddsEnvironmentVariablesIfADefaultEnvFileExists()
        {
            Action action = () => DotEnv.Config();
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("hello").Should().Be("world");
        }

        [Fact]
        public void AddsEnvironmentVariablesAndSetsValueAsNullIfNoneExists()
        {
            Action action = () => DotEnv.Config();
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("strongestavenger").Should().Be(null);
        }

        [Fact]
        public void AllowsEnvFilePathToBeSpecified()
        {
            Action action = () => DotEnv.Config(true, ValuesAndCommentsEnvFileName);
            action.ShouldNotThrow();

            Environment.GetEnvironmentVariable("me").Should().Be("winner");
        }

        [Fact]
        public void ShouldReturnUntrimmedValuesWhenTrimIsFalse()
        {
            DotEnv.Config(true, WhitespacesEnvFileName, Encoding.UTF8, false);

            Environment.GetEnvironmentVariable("DB_CONNECTION").Should().Be("mysql  ");
            Environment.GetEnvironmentVariable("DB_HOST").Should().Be("127.0.0.1");
            Environment.GetEnvironmentVariable("DB_PORT").Should().Be("  3306");
            Environment.GetEnvironmentVariable("DB_DATABASE").Should().Be("laravel");
        }

        [Fact]
        public void ShouldReturnTrimmedValuesWhenTrimIsTrue()
        {
            DotEnv.Config(true, WhitespacesEnvFileName, Encoding.UTF8, true);

            Environment.GetEnvironmentVariable("DB_CONNECTION").Should().Be("mysql");
            Environment.GetEnvironmentVariable("DB_HOST").Should().Be("127.0.0.1");
            Environment.GetEnvironmentVariable("DB_PORT").Should().Be("3306");
            Environment.GetEnvironmentVariable("DB_DATABASE").Should().Be("laravel");
        }
    }
}