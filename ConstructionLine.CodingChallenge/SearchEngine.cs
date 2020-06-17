using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;
        private readonly Dictionary<Size, Dictionary<Color, List<Shirt>>> _lookup = new Dictionary<Size, Dictionary<Color, List<Shirt>>>();
        private readonly ILookup<Color, Shirt> _colourLookup;
        private readonly ILookup<Size, Shirt> _sizeLookup;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;

            foreach (var size in Size.All)
            {
                _lookup[size] = new Dictionary<Color, List<Shirt>>();
                foreach (var color in Color.All)
                {
                    _lookup[size][color] = new List<Shirt>();
                }
            }

            foreach (var shirt in shirts)
            {
                _lookup[shirt.Size][shirt.Color].Add(shirt);
            }

            _colourLookup = shirts.ToLookup(s => s.Color);
            _sizeLookup = shirts.ToLookup(s => s.Size);
        }


        public SearchResults Search(SearchOptions options)
        {
            var searchResults = new SearchResults
            {
                SizeCounts = new List<SizeCount>(),
                ColorCounts = new List<ColorCount>(),
            };

            foreach (var size in Size.All)
            {
                searchResults.SizeCounts.Add(new SizeCount { Size = size });
            }

            foreach (var color in Color.All)
            {
                searchResults.ColorCounts.Add(new ColorCount { Color = color });
            }

            var matches = new Dictionary<Guid, Shirt>();

            // Find results were colour & size options are specified
            foreach (var optionsColor in options.Colors)
            {
                foreach (var optionsSize in options.Sizes)
                {
                    foreach (var shirt in _lookup[optionsSize][optionsColor])
                    {
                        matches[shirt.Id] = shirt;
                        searchResults.SizeCounts.Single(x => Equals(x.Size, optionsSize)).Count++;
                        searchResults.ColorCounts.Single(x => Equals(x.Color, optionsColor)).Count++;
                    }
                }
            }

            // Find results where only colour options are specified
            if (!options.Sizes.Any())
            {
                foreach (var optionsColor in options.Colors)
                {
                    foreach (var shirt in _colourLookup[optionsColor])
                    {
                        matches[shirt.Id] = shirt;

                        searchResults.ColorCounts.Single(x => Equals(x.Color, optionsColor)).Count++;
                    }
                }
            }

            // Find results where only size options are specified
            if (!options.Colors.Any())
            {
                foreach (var optionsSize in options.Sizes)
                {
                    foreach (var shirt in _sizeLookup[optionsSize])
                    {
                        matches[shirt.Id] = shirt;

                        searchResults.SizeCounts.Single(x => Equals(x.Size, optionsSize)).Count++;
                    }
                }
            }

            searchResults.Shirts = matches.Values.ToList();

            return searchResults;
        }
    }
}