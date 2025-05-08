using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XICore
{
    public class BCNetwork
    {
    }
    public class AMChartDto
    {
        public string name { get; set; }
        public int value { get; set; }
        public List<BCChild> children { get; set; }
    }

    public class BCChild
    {
        public string name { get; set; }
        //public List<string> linkWith { get; set; }
        public string linkWith { get; set; }
        public List<BCChild> children { get; set; }
        public int? value { get; set; }
    }
    public class AIRecomendations
    {
        public string systemInput { get; set; }
        public string userInput { get; set; }
    }
    public class AIRecomendationsResponse
    {
        public string vulnerability { get; set; }
        public string description { get; set; }
        public string recommendation { get; set; }
        public int line_number { get; set; }
        public string swc_id { get; set; }
        public string code { get; set; }
        public string example_solution { get; set; }
        public string criticality_score { get; set; }
    }
}