using MailTemplate.Worker.Commands;
using RazorLight;
using System.Reflection;
using MailTemplate.Worker.Base;

namespace MailTemplate.Worker.Services;

public class EmailGenerator : IEmailGenerator
{
    private readonly IRazorLightEngine _engine;

    public EmailGenerator(IRazorLightEngine engine)
    {
        this._engine = engine;
    }

    private PropertyInfo[] GetModelPropertyInfo<TModel>() => typeof(TModel).GetProperties();

    private T CreateBodyModel<T>(SendEmailCommand command)
    {
        var properties = this.GetModelPropertyInfo<T>();

        var model = Activator.CreateInstance<T>();

        foreach (var propertyInfo in properties)
        {
            if (command.BodyProperties.TryGetValue(propertyInfo.Name, out var value))
            {
                propertyInfo.SetValue(model, value);
            }
        }

        return model;
    }

    private T CreateSubjectModel<T>(SendEmailCommand command)
    {
        var properties = this.GetModelPropertyInfo<T>();

        var model = Activator.CreateInstance<T>();

        foreach (var propertyInfo in properties)
        {
            if (command.SubjectProperties.TryGetValue(propertyInfo.Name, out var value))
            {
                propertyInfo.SetValue(model, value);
            }
        }

        return model;
    }

    public Task<string> GenerateBody<T>(SendEmailCommand command) where T : TemplateModel =>
        this._engine.CompileRenderAsync($"{command.Type}.cshtml", CreateBodyModel<T>(command));

    public Task<string> GenerateSubject<T>(SendEmailCommand command) where T : TemplateSubjectModel =>
        this._engine.CompileRenderAsync($"Subjects/{command.Type}Subject.cshtml", CreateSubjectModel<T>(command));
}

public interface IEmailGenerator
{
    Task<string> GenerateBody<T>(SendEmailCommand command) where T : TemplateModel;

    Task<string> GenerateSubject<T>(SendEmailCommand command) where T : TemplateSubjectModel;
}