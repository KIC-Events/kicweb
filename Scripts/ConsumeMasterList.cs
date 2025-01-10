using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using KiCData.Models;

namespace Scripts;

public class ConsumeMasterList
{
	private KiCdbContext _context;
	List<MasterListObj> masterListObjs;
	
	public ConsumeMasterList(KiCdbContext context, string path)
	{
		_context = context;
		
		if(!File.Exists(path))
		{
			Console.WriteLine("File not found!");
			Environment.Exit(1);
		}
		
		try
		{
			masterListObjs = JsonSerializer.Deserialize<List<MasterListObj>>(path);
		}
		catch(Exception ex)
		{
			Console.WriteLine(ex.Message);
			Environment.Exit(2);
		}
	}
	
	public void Execute()
	{
		foreach(MasterListObj m in masterListObjs)
		{
			Console.WriteLine("Updating attendee " + m.confNumber.ToString());
			
			Attendee a = _context.Attendees
				.Where(a => a.Id == int.Parse(m.confNumber))
				.First();
				
			if(m.bcComplete != "No" && m.bcComplete is not null)
			{
				a.BackgroundChecked = true;
			}
			
			if(m.paymentConfirmed != "No" && m.paymentConfirmed is not null)
			{
				a.IsPaid = true;
			}
			
			if(m.pronouns is not null)
			{
				a.Pronouns = m.pronouns;
			}
		}
		
		Console.WriteLine("Saving changes to DB.");
		_context.SaveChanges();
	}
}

public class MasterListObj
{
	public string? confNumber{get;set;}
	public string? bcComplete{get;set;}
	public string? paymentConfirmed{get;set;}
	public string? pronouns{get;set;}
}
