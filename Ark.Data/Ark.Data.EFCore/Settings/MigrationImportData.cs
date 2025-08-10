using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Ark;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ark.Data.EFCore
{
    /// <summary>
    /// Migration for importing data from JSON files.
    /// </summary>
    public partial class ImportData : Migration
    {
        /// <summary>
        /// Applies the migration by importing data from JSON files.
        /// </summary>
        /// <param name="migrationBuilder">The builder used to create the migration.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            var jsonFiles = Directory.GetFiles(dataDirectory, "*.json");

            foreach (var jsonFile in jsonFiles)
            {
                var tableName = Path.GetFileNameWithoutExtension(jsonFile);
                var entityType = GetEntityTypeByName(tableName);

                if (entityType != null)
                {
                    var records = LoadData(jsonFile, entityType);

                    foreach (var record in records)
                    {
                        migrationBuilder.InsertData(
                            table: tableName,
                            columns: GetPropertyNames(entityType),
                            values: GetPropertyValues(record, entityType)
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Reverts the migration by deleting imported data.
        /// </summary>
        /// <param name="migrationBuilder">The builder used to create the migration.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            var jsonFiles = Directory.GetFiles(dataDirectory, "*.json");

            foreach (var jsonFile in jsonFiles)
            {
                var tableName = Path.GetFileNameWithoutExtension(jsonFile);
                migrationBuilder.Sql($"DELETE FROM {tableName}");
            }
        }

        /// <summary>
        /// Loads data from a JSON file into a list of objects of the specified type.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <param name="entityType">The type of the entity.</param>
        /// <returns>A list of objects deserialized from the JSON file.</returns>
        private List<object> LoadData(string filePath, Type entityType)
        {
            var jsonString = File.ReadAllText(filePath);
            var genericListType = typeof(List<>).MakeGenericType(entityType);
            return (List<object>)JsonSerializer.Deserialize(jsonString, genericListType);
        }

        /// <summary>
        /// Gets the entity type by name from the current assembly.
        /// </summary>
        /// <param name="name">The name of the entity type.</param>
        /// <returns>The entity type if found; otherwise, null.</returns>
        private Type GetEntityTypeByName(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetType($"YourNamespace.Models.{name}");
        }

        /// <summary>
        /// Gets the property names of the entity type.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>An array of property names.</returns>
        private string[] GetPropertyNames(Type entityType)
        {
            var properties = entityType.GetProperties();
            var propertyNames = new List<string>();
            foreach (var property in properties)
            {
                propertyNames.Add(property.Name);
            }
            return propertyNames.ToArray();
        }

        /// <summary>
        /// Gets the property values of the entity object.
        /// </summary>
        /// <param name="entity">The entity object.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>An array of property values.</returns>
        private object[] GetPropertyValues(object entity, Type entityType)
        {
            var properties = entityType.GetProperties();
            var propertyValues = new List<object>();
            foreach (var property in properties)
            {
                propertyValues.Add(property.GetValue(entity));
            }
            return propertyValues.ToArray();
        }
    }
}
