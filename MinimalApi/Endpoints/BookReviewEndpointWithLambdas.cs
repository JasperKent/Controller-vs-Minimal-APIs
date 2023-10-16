using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Endpoints
{
    public static class BookReviewEndpointWithLambdas
    {
        private static bool ValidateReview(BookReview bookReview)
        {
            return !string.IsNullOrWhiteSpace(bookReview.Title)
                && bookReview.Rating >= 1
                && bookReview.Rating <= 5;
        }

        public static IEndpointRouteBuilder MapBookReviewsWithLambdas(this IEndpointRouteBuilder builder)
        {
            var groupBuilder = builder.MapGroup("api/reviews");

            groupBuilder.MapGet("/", async (ReviewContext reviewContext) => TypedResults.Ok(await reviewContext.BookReviews.ToArrayAsync()));

            groupBuilder.MapGet("/{id:int}", async Task<IResult> (ReviewContext reviewContext, int id) =>
            {
                var result = await reviewContext.BookReviews.FindAsync(id);

                if (result == null)
                    return TypedResults.NotFound();
                else
                    return TypedResults.Ok(result);
            }).WithName("GetOne");

            groupBuilder.MapGet("/Summary", async (ReviewContext reviewContext) =>
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
            });

            groupBuilder.MapPost("/", async Task<IResult> (ReviewContext reviewContext, [FromBody] BookReview review) =>
            {
                if (!ValidateReview(review))
                    return TypedResults.UnprocessableEntity();

                reviewContext.BookReviews.Add(review);

                await reviewContext.SaveChangesAsync();

                return TypedResults.CreatedAtRoute(
                    routeName: "GetOne",
                    routeValues: new { id = review.Id },
                    value: review);
            });

            groupBuilder.MapPut("/{id:int}", async Task<IResult> (ReviewContext reviewContext, int id, [FromBody] BookReview review) =>
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
            });

            groupBuilder.MapDelete("/{id:int}", async Task<IResult> (ReviewContext reviewContext, int id) =>
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
            });

            return builder;
        }
    }
}
