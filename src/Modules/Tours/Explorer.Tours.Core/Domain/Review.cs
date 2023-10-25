﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class Review : Entity
    {
        public int Rating { get; init; }
        public string Comment { get; init; }
        public long TouristId { get; init; }
        public DateOnly TourVisitDate { get; init; }
        public DateOnly CommentDate { get; init; }
        public int TourId { get; init; }
        public List<string> Images { get; init; }
        public Review(int rating, string comment, long touristId, DateOnly tourVisitDate, DateOnly commentDate, int tourId, List<string> images)
        {
            if (rating < 1 || rating > 5) throw new ArgumentException("Invalid rating.");
            if (string.IsNullOrWhiteSpace(comment)) throw new ArgumentException("Invalid comment.");
            if (images.Count < 1) throw new ArgumentException("Invalid images input.");
            

            Rating = rating;
            Comment = comment;
            TouristId = touristId;
            TourVisitDate = tourVisitDate;
            CommentDate = commentDate;
            TourId = tourId;
            Images = images;
        }
    }
}
