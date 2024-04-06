using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassIn.Application.UseCases.Events.Register;
public class RegisterEventUseCase
{
    public ResponseRegisteredEventJson Execute(RequestEventJson request)
    {
        Validate(request);
        var dbContext = new PassInDbContext();

        var entity = new Event
        {
            Title = request.Title,
            Details = request.Details,
            Maximum_Attendees = request.MaximumAttendees,
            Slug = request.Title.ToLower().Replace(" ", "-")

        };
        dbContext.Events.Add(entity);
        dbContext.SaveChanges();

        return new ResponseRegisteredEventJson
        {
            Id = entity.Id,
        };

    }

    public void Validate(RequestEventJson request)
    {
        if(request.MaximumAttendees <= 0)
        {
            throw new PassInException("The Maximum attendees is invalida.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new PassInException("The title is invalid.");
        }

        if (string.IsNullOrWhiteSpace(request.Details))
        {
            throw new PassInException("The details is needed.");
        }
    }
}
