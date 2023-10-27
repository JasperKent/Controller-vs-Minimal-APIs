using DataAccess;
using Microsoft.AspNetCore.Http.HttpResults;
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

            groupBuilder.MapGet("/", async (ReviewContext reviewContext) 
                => TypedResults.Ok(await reviewContext.BookReviews.ToArrayAsync()))
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Get")
            .WithOpenApi(op =>
            {
                op.Summary = "Get all book reviews";

                return op;
            });

            groupBuilder.MapGet("/{id:int}", async Task<Results<Ok<BookReview>, NotFound>> (ReviewContext reviewContext, int id) =>
            {
                var result = await reviewContext.BookReviews.FindAsync(id);

                if (result == null)
                    return TypedResults.NotFound();
                else
                    return TypedResults.Ok(result);
            })
            .WithName("GetOne")
            //.Produces(StatusCodes.Status200OK)
            //.Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            //.WithSummary("Gets a single book review by id")
            .WithTags("Get")
            .WithOpenApi(op =>
            {
                op.Summary = "Gets a single book review by id";
                op.Description = "More detailed description";

                op.Parameters[0].Description = "The id of the book review";

                return op;
            });

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
            })
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Get")
            .WithOpenApi(op =>
            {
                op.Summary = "Gets a summary of the reviews for each book with an average rating";

                return op;
            });

            groupBuilder.MapPost("/", async Task<Results<UnprocessableEntity, 
                                                 CreatedAtRoute<BookReview>,
                                                 BadRequest>> 
                                      (ReviewContext reviewContext, [FromBody] BookReview review) =>
            {
                if (!ValidateReview(review))
                    return TypedResults.UnprocessableEntity();

                reviewContext.BookReviews.Add(review);

                await reviewContext.SaveChangesAsync();

                return TypedResults.CreatedAtRoute(
                    routeName: "GetOne",
                    routeValues: new { id = review.Id },
                    value: review);
            })
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Post")
            .WithOpenApi(op =>
            {
                op.Summary = "Creates a new book review";

                op.RequestBody.Description = "The new book review";

                return op;
            });

            groupBuilder.MapPut("/{id:int}", async Task<Results<UnprocessableEntity, NotFound, Ok, BadRequest>> 
                                             (ReviewContext reviewContext, int id, [FromBody] BookReview review) =>
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
            })
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Put")
            .WithOpenApi(op =>
            {
                op.Summary = "Modifies and existing book review";
                op.Parameters[0].Description = "The id of the review to modify";
                op.RequestBody.Description = "The updated review";

                return op;
            });

            groupBuilder.MapDelete("/{id:int}", async Task<Results<Ok, NotFound>> 
                                                (ReviewContext reviewContext, int id) =>
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
            })
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Delete")
            .WithOpenApi(op =>
            {
                op.Summary = "Deletes a book review";
                op.Parameters[0].Description = "The id of the review to delete";

                return op;
            });

            return builder;
        }
    }
}
