using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;
        private ILookup<Color, Shirt> _colourLookup;
        private ILookup<Size, Shirt> _sizeLookup;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;

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

            foreach (var optionsColor in options.Colors)
            {
                foreach (var shirt in _colourLookup[optionsColor])
                {
                    matches[shirt.Id] = shirt;

                    searchResults.ColorCounts.Single(x => x.Color == optionsColor).Count++;
                }
            }

            foreach (var optionsSize in options.Sizes)
            {
                foreach (var shirt in _sizeLookup[optionsSize])
                {
                    matches[shirt.Id] = shirt;

                    searchResults.SizeCounts.Single(x => x.Size == optionsSize).Count++;
                }
            }

            searchResults.Shirts = matches.Values.ToList();

            return searchResults;
        }
    }
}