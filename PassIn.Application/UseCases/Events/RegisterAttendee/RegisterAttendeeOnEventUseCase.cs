using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;
using System.Net.Mail;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;
public class RegisterAttendeeOnEventUseCase
{
    private readonly PassInDbContext _dbContext;

    public RegisterAttendeeOnEventUseCase()
    {
        _dbContext = new PassInDbContext();
    }
    public ResponseRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
    {
        Validate(eventId,request);
        var entity = new Attendee
        {
            Email = request.Email,
            Name = request.Name,
            Event_Id = eventId,
            Created_At = DateTime.UtcNow
        };
        _dbContext.Attendees.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Id
        };
    }

    private void Validate(Guid eventId, RequestRegisterEventJson request) 
    {
        var eventEntity = _dbContext.Events.Find(eventId);

        if (eventEntity is null)
            throw new NotFoundException("An event with this id doesnt exist.");

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ErrorOnValidationException("The name is invalid.");
        }

        var emailIsValid = EmailIsvalid(request.Email);

        if(emailIsValid == false)
        {
            throw new ErrorOnValidationException("The email is invalid!!!");
        }

        var attendeeAlreadyRegistrered = _dbContext
            .Attendees
            .Any(attendee => attendee.Email == request.Email && attendee.Event_Id == eventId);

        if(attendeeAlreadyRegistrered)
        {
            throw new ConflictException("You can not register twice on the same event.");
        }

        var attendeesForEvent = _dbContext.Attendees.Count(x => x.Event_Id == eventId);
        if(attendeesForEvent >= eventEntity.Maximum_Attendees)
            throw new ErrorOnValidationException("There is no room for this event!");
    }

    private bool EmailIsvalid(string email)
    {
        try
        {
            new MailAddress(email);

            return true;
        }
        catch
        {
            return false;
        }
        
    }
}
