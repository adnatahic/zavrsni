#region Using

using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Timers;

#endregion

public class DosAttackModule : IHttpModule
{

  

    void IHttpModule.Dispose()
    {
        // Nothing to dispose; 
    }

    void IHttpModule.Init(HttpApplication context)
    {
        context.BeginRequest += new EventHandler(context_BeginRequest);
    }
    
    private static Dictionary<string, short> ipAdrese = new Dictionary<string, short>();
    private static Stack<string> blokirane = new Stack<string>();
    private static Timer timer = CreateTimer();
    private static Timer blokiranVrijeme = CreateBanningTimer();

 

    private const int blokiranzahtjev = 10;
    private const int blokiranInterval = 1000;
    private const int pustanje = 5 * 60 * 1000;

    private void context_BeginRequest(object sender, EventArgs e)
    {
        string ip = HttpContext.Current.Request.UserHostAddress;
        if (blokirane.Contains(ip))
        {
            HttpContext.Current.Response.StatusCode = 403;
            HttpContext.Current.Response.End();
        }

        CheckIpAddress(ip);
    }


    private static void CheckIpAddress(string ip)
    {
        if (!ipAdrese.ContainsKey(ip))
        {
            ipAdrese[ip] = 1;
        }
        else if (ipAdrese[ip] == blokiranzahtjev)
        {
            blokirane.Push(ip);
            ipAdrese.Remove(ip);
        }
        else
        {
            ipAdrese[ip]++;
        }
    }



    private static Timer CreateTimer()
    {
        Timer timer = GetTimer(blokiranInterval);
        timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        return timer;
    }


    private static Timer CreateBanningTimer()
    {
        Timer timer = GetTimer(pustanje);
        timer.Elapsed += delegate { blokirane.Pop(); };
        return timer;
    }


    private static Timer GetTimer(int interval)
    {
        Timer timer = new Timer();
        timer.Interval = interval;
        timer.Start();

        return timer;
    }

    private static void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        foreach (string key in ipAdrese.Keys)
        {
            ipAdrese[key]--;
            if (ipAdrese[key] == 0)
                ipAdrese.Remove(key);
        }
    }



}