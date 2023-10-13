using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Endpoints
{
    public static class BookReviewEndpoints
    {
        private static bool ValidateReview(BookReview bookReview)
        {
            return !string.IsNullOrWhiteSpace(bookReview.Title)
                && bookReview.Rating >= 1
                && bookReview.Rating <= 5;
        }

        public static async Task<IResult> GetAll(ReviewContext reviewContext)
        {
            return TypedResults.Ok(await reviewContext.BookReviews.ToArrayAsync());
        }

        public static async Task<IResult> Summary(ReviewContext reviewContext)
        {
            var summaries = await reviewContext.BookReviews.GroupBy(r => r.Title).Select(g =>
            new BookReview
            {
                Title = g.Key,
                Rating = Math.Round(g.Average(r => r.Rating), 2)
            }).ToListAsync();

            int id = 1;

            summaries.ForEach(r => r.Id = id++);

            return TypedResults.Ok(summaries);
        }

        public static async Task<IResult> GetOne(ReviewContext reviewContext, int id)
        {
            var result = await reviewContext.BookReviews.FindAsync(id);

            if (result == null)
                return TypedResults.NotFound();
            else
                return TypedResults.Ok(result);
        }

        public static async Task<IResult> Post(ReviewContext reviewContext, [FromBody] BookReview review)
        {
            if (!ValidateReview(review))
                return TypedResults.UnprocessableEntity();

            reviewContext.BookReviews.Add(review);

            await reviewContext.SaveChangesAsync();

            return TypedResults.CreatedAtRoute(
                routeName: "GetOne", 
                routeValues: new { id = review.Id }, 
                value: review);
        }

        public static async Task<IResult> Put(ReviewContext reviewContext, int id, [FromBody] BookReview review)
        {
            if (!ValidateReview(review))
                return TypedResults.UnprocessableEntity();

            var result = await reviewContext.BookReviews.FindAsync(id); 

            if (result == null)
                return TypedResults.NotFound();
            else
            {
                result.Rating = review.Rating;
                result.Title = review.Title;

                await reviewContext.SaveChangesAsync();

                return TypedResults.Ok();
            }
        }

        public static async Task<IResult> Delete(ReviewContext reviewContext, int id)
        {
            var result = await reviewContext.BookReviews.FindAsync(id);

            if (result == null)
                return TypedResults.NotFound();
            else
            {
                reviewContext.BookReviews.Remove(result);

                await reviewContext.SaveChangesAsync();

                return TypedResults.Ok();
            }
        }
    }
}
