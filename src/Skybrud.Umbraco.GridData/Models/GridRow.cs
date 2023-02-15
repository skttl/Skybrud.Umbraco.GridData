﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Skybrud.Umbraco.GridData.Factories;

namespace Skybrud.Umbraco.GridData.Models {

    /// <summary>
    /// Class representing a row in an Umbraco Grid.
    /// </summary>
    public class GridRow : GridElement {

        #region Properties

        /// <summary>
        /// Gets a reference to the parent <see cref="GridSection"/>.
        /// </summary>
        public GridSection Section { get; }

        /// <summary>
        /// Gets the unique ID of the row.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the label of the row. Use <see cref="HasLabel"/> to check whether a label has been specified.
        /// </summary>
        public string? Label { get; }

        /// <summary>
        /// Gets whether a label has been specified for the definition of this row.
        /// </summary>
        public bool HasLabel => string.IsNullOrWhiteSpace(Label) == false;

        /// <summary>
        /// Gets the name of the row.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets an array of all areas in the row.
        /// </summary>
        public IReadOnlyList<GridArea> Areas { get; }

        /// <summary>
        /// Gets a reference to the previous row.
        /// </summary>
        public GridRow? PreviousRow { get; internal set; }

        /// <summary>
        /// Gets a reference to the next row.
        /// </summary>
        public GridRow? NextRow { get; internal set; }

        /// <summary>
        /// Gets whether the row has any areas.
        /// </summary>
        public bool HasAreas => Areas.Count > 0;

        /// <summary>
        /// Gets the first area of the row. If the row doesn't contain any areas, this property will return <c>null</c>.
        /// </summary>
        public GridArea? FirstRow => Areas.FirstOrDefault();

        /// <summary>
        /// Gets the last area of the row. If the row doesn't contain any areas, this property will return <c>null</c>.
        /// </summary>
        public GridArea? LastRow => Areas.LastOrDefault();

        /// <summary>
        /// Gets whether at least one area or control within the row is valid.
        /// </summary>
        public override bool IsValid {
            get { return Areas.Any(x => x.IsValid); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="json"/> object, <paramref name="section"/> and <paramref name="factory"/>.
        /// </summary>
        /// <param name="json">An instance of <see cref="JObject"/> representing the section.</param>
        /// <param name="section">The parent section.</param>
        /// <param name="factory">The factory used for parsing subsequent parts of the grid.</param>
        public GridRow(JObject json, GridSection section, IGridFactory factory) : base(json) {

            Section = section;
            Id = json.GetString("id")!;
            Label = json.GetString("label");
            Name = json.GetString("name")!;

            Areas = json.GetArray("areas", x => factory.CreateGridArea(x, this)) ?? Array.Empty<GridArea>();

            // Update "PreviousArea" and "NextArea" properties
            for (int i = 1; i < Areas.Count; i++) {
                Areas[i - 1].NextArea = Areas[i];
                Areas[i].PreviousArea = Areas[i - 1];
            }

        }

        #endregion

        #region Member methods

        /// <summary>
        /// Gets an array of all nested controls.
        /// </summary>
        public GridControl[] GetAllControls() {
            return (
                from area in Areas
                from control in area.Controls
                select control
            ).ToArray();
        }

        /// <summary>
        /// Gets an array of all nested controls with the specified editor <paramref name="alias"/>.
        /// </summary>
        /// <param name="alias">The editor alias of controls to be returned.</param>
        public GridControl[] GetAllControls(string alias) {
            return GetAllControls(x => x.Editor.Alias == alias);
        }

        /// <summary>
        /// Gets an array of all nested controls matching the specified <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The predicate (callback function) used for comparison.</param>
        public GridControl[] GetAllControls(Func<GridControl, bool> predicate) {
            return (
                from area in Areas
                from control in area.Controls
                where predicate(control)
                select control
            ).ToArray();
        }

        /// <summary>
        /// Writes a string representation of the row to <paramref name="writer"/>.
        /// </summary>
        /// <param name="context">The current grid context.</param>
        /// <param name="writer">The writer.</param>
        public override void WriteSearchableText(GridContext context, TextWriter writer) {
            foreach (GridArea area in Areas) area.WriteSearchableText(context, writer);
        }

        #endregion

    }

}