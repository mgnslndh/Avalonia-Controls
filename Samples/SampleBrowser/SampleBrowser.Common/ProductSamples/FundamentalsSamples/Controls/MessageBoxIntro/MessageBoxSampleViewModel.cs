﻿using ActiproSoftware.UI.Avalonia.Controls;
using System;
using System.ComponentModel;
using System.Text;

namespace ActiproSoftware.ProductSamples.FundamentalsSamples.Controls.MessageBoxIntro {

	/// <summary>
	/// Defines a view model for working with the <see cref="MessageBox"/> sample.
	/// </summary>
	/// <remarks>This is NOT a general view model for a MessageBox.</remarks>
	public class MessageBoxSampleViewModel : ObservableObjectBase {

		private bool _addHelpButton = false;
		private MessageBoxButtons _buttons = MessageBoxButtons.OK;
		private string _caption = string.Empty;
		private MessageBoxResult _defaultResult = MessageBoxResult.None;
		private MessageBoxImage _image = MessageBoxImage.Information;
		private MessageBoxResult _result = MessageBoxResult.None;
		private string _sampleCode = string.Empty;
		private string _text = string.Empty;

		/////////////////////////////////////////////////////////////////////////////////////////////////////
		// OBJECT
		/////////////////////////////////////////////////////////////////////////////////////////////////////

		public MessageBoxSampleViewModel() {
			this.Caption = "Message title";
			this.Text = "This is the message that will be displayed.";
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////
		// NON-PUBLIC PROCEDURES
		/////////////////////////////////////////////////////////////////////////////////////////////////////

		private void UpdateSampleCode() {
			bool hasImage = Image != MessageBoxImage.None;
			bool hasDefaultResult = DefaultResult != MessageBoxResult.None;
			bool hasHelpButton = AddHelpButton;

			string indent = new string(' ', 4);
			var sample = new StringBuilder()
				.AppendLine($"var result = await {nameof(MessageBox)}.{nameof(MessageBox.Show)}(")
				.AppendLine(indent + FormatAsString(Text) + ",")
				.AppendLine(indent + FormatAsString(Caption) + ",")
				.Append(indent + nameof(MessageBoxButtons) + "." + Buttons);

			if (hasImage || hasDefaultResult || hasHelpButton) {
				sample.AppendLine(",")
					.Append(indent + nameof(MessageBoxImage) + "." + Image);
				if (hasDefaultResult || hasHelpButton) {
					sample.AppendLine(",")
						.Append(indent + nameof(MessageBoxResult) + "." + DefaultResult);
					if (hasHelpButton) {
						sample.AppendLine(",")
							.AppendLine(indent + "(builder) => builder")
							.AppendLine(indent + indent + ".WithHelpCommand(() => {")
							.AppendLine(indent + indent + indent + "// Define action to be invoked when Help is clicked")
							.Append(indent + indent + "}");
					}
				}
			}
			sample.AppendLine()
				.AppendLine(");");

			this.SampleCode = sample.ToString();

			static string FormatAsString(string input) {
				if (input is null)
					return "null";
				return "\"" + input.Replace("\"", "\\\"")
					.Replace("\t", "\\\t")
					.Replace("\n", "\\\n")
					.Replace("\r", "\\\r") + "\"";
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////
		// PUBLIC PROCEDURES
		/////////////////////////////////////////////////////////////////////////////////////////////////////

		public bool AddHelpButton {
			get => _addHelpButton;
			set {
				if (SetProperty(ref _addHelpButton, value))
					OnPropertyChanged(nameof(ButtonsResolved));
			}
		}

		public MessageBoxButtons Buttons {
			get => _buttons;
			set {
				if (SetProperty(ref _buttons, value)) {
					OnPropertyChanged(nameof(ButtonsResolved));
					OnPropertyChanged(nameof(HasCloseButton));
				}
			} 
		}

		public MessageBoxButtons ButtonsResolved {
			get {
				var buttons = this.Buttons;
				if (AddHelpButton)
					buttons |= MessageBoxButtons.Help;
				return buttons;
			}
		}

		public string Caption {
			get => _caption;
			set => SetProperty(ref _caption, value ?? string.Empty);
		}

		public MessageBoxResult DefaultResult {
			get => _defaultResult;
			set => SetProperty(ref _defaultResult, value);
		}

		public bool HasCloseButton
			=> (this.Buttons == MessageBoxButtons.OK
				|| this.Buttons.HasFlag(MessageBoxButtons.Cancel)
				|| this.Buttons.HasFlag(MessageBoxButtons.Ignore));

		public MessageBoxImage Image {
			get => _image;
			set => SetProperty(ref _image, value);
		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);

			UpdateSampleCode();
		}

		public MessageBoxResult Result {
			get => _result;
			private set => SetProperty(ref _result, value);
		}

		public string Text {
			get => _text;
			set => SetProperty(ref _text, value ?? string.Empty);
		}

		public async void ShowMessageBox() {
			if (AddHelpButton) {
				Result = await MessageBox.Show(Text, Caption, Buttons, Image, DefaultResult, (builder) => builder
					.WithHelpCommand(() => MessageBox.Show("Here is where you would show contextual help.", "Help"))
				);
			}
			else {
				Result = await MessageBox.Show(Text, Caption, Buttons, Image, DefaultResult);
			}
		}

		public string SampleCode {
			get => _sampleCode;
			private set => SetProperty(ref _sampleCode, value ?? string.Empty);
		}

	}

}
