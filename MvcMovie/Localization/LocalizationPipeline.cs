﻿using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMovie.Localization
{
    public class LocalizationPipeline
    {
        public void Configure(IApplicationBuilder app, RequestLocalizationOptions options)
        {
            app.UseRequestLocalization(options);
        }
    }
}

