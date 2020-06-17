using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;
        private readonly Dictionary<Size, Dictionary<Color, List<Shirt>>> lookup = new Dictionary<Size, Dictionary<Color, List<Shirt>>>();

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;

            foreach (var size in Size.All)
            {
                lookup[size] = new Dictionary<Color, List<Shirt>>();
                foreach (var color in Color.All)
                {
                    lookup[size][color] = new List<Shirt>();
                }
            }

            foreach (var shirt in shirts)
            {
                lookup[shirt.Size][shirt.Color].Add(shirt);
            }
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

            foreach (var optionsColor in options.Colors)
            {
                foreach (var optionsSize in options.Sizes)
                {
                    foreach (var shirt in lookup[optionsSize][optionsColor])
                    {
                        matches[shirt.Id] = shirt;
                        searchResults.SizeCounts.Single(x => Equals(x.Size, optionsSize)).Count++;
                        searchResults.ColorCounts.Single(x => Equals(x.Color, optionsColor)).Count++;
                    }
                }
            }


            searchResults.Shirts = matches.Values.ToList();

            return searchResults;
        }
    }
}