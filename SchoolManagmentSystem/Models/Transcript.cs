using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
	public enum Grade
	{
		A, B, C, D, E, F
	}

	public class Transcript
	{
		[Key]
		public int ID { get; set; }
		public Grade Grade { get; set; }

		// Relationship
		public int StudentID { get; set; }
		[ForeignKey("StudentID")]
		public Student? Student { get; set; }

		//Relationship
		public int CourseID { get; set; }
		[ForeignKey("CourseID")]
		public Course? Course { get; set; }

	}
}

