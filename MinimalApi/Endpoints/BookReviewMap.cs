namespace MinimalApi.Endpoints
{
    public static class BookReviewMap
    {
        public static IEndpointRouteBuilder MapBookReviews(this IEndpointRouteBuilder builder) 
        {
            var groupBuilder = builder.MapGroup("api/reviews");

            groupBuilder.MapGet("/", BookReviewEndpoints.GetAll);
            groupBuilder.MapGet("/{id:int}", BookReviewEndpoints.GetOne).WithName("GetOne");
            groupBuilder.MapGet("/Summary", BookReviewEndpoints.Summary);
            groupBuilder.MapPost("/", BookReviewEndpoints.Post);
            groupBuilder.MapPut("/{id:int}", BookReviewEndpoints.Put);
            groupBuilder.MapDelete("/{id:int}", BookReviewEndpoints.Delete);

            return builder;
        }
    }
}
