using System;
using FakeHttpContext.Switchers;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace FakeHttpContext.Tests.Switchers
{
    public class PrivateFieldSwitcherTests
    {
        private object _testField;

        [Theory, AutoData]
        public void Should_set_field_value(object newValue)
        {
            // Act
            using (new PrivateFieldSwitcher(this, "_testField", newValue))
            {
                // Assert
                _testField.Should().BeSameAs(newValue);
            }
        }

        [Theory, AutoData]
        public void Should_restore_field_value(object value, object newValue)
        {
            // Arrange
            _testField = value;

            // Act
            using (new PrivateFieldSwitcher(this, "_testField", newValue))
            {
            }

            // Assert
            _testField.Should().BeSameAs(value);
        }

        [Theory, AutoData]
        public void Should_have_getters(object newValue)
        {
            // Act
            using (var switcher = new PrivateFieldSwitcher(this, "_testField", newValue))
            {
                // Assert
                switcher.FieldName.Should().Be("_testField");
                switcher.Tartget.Should().BeSameAs(this);
                switcher.NewValue.Should().Be(newValue);
            }
        }

        [Theory, AutoData]
        public void Should_throw_if_field_not_found(object newValue)
        {
            // Act
            Action action = () => { new PrivateFieldSwitcher(this, "nonExistingField", newValue); };

            // Assert
            action.ShouldThrow<InvalidOperationException>()
                .WithMessage(string.Format("Can't find field 'nonExistingField' in the type '{0}'", GetType()));
        }
    }
}