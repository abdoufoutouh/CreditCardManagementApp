<template>
  <div class="credit-card-form">
    <div class="form-container">
      <div class="form-section">
        <h2 class="form-title">Create New Credit Card</h2>
        
        <form class="form" @submit.prevent>
          <div class="form-row">
            <div class="form-group full-width">
              <label for="cardNumber" class="form-label">Card Number</label>
              <div class="input-with-button">
                <input
                  id="cardNumber"
                  v-model="formData.cardNumber"
                  @input="formatCardNumber"
                  @blur="validateCardNumber"
                  type="text"
                  maxlength="19"
                  placeholder="1234 5678 1234 5678"
                  class="form-input"
                  :class="{ 'error': errors.cardNumber }"
                />
                <button
                  type="button"
                  @click="handleGenerateCardNumber"
                  class="generate-button"
                  :disabled="!formData.type || isGenerating"
                  title="Generate a valid card number"
                >
                  {{ isGenerating ? '⏳' : '🎲' }} Generate
                </button>
              </div>
              <span v-if="errors.cardNumber" class="error-message">{{ errors.cardNumber }}</span>
              <span v-if="!formData.type" class="info-message">Select a card type first to generate</span>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="expirationDate" class="form-label">Expiration Date</label>
              <input
                id="expirationDate"
                v-model="formData.expirationDate"
                @blur="validateExpirationDate"
                type="month"
                :min="getCurrentMonth()"
                :max="getMaxMonth()"
                class="form-input"
                :class="{ 'error': errors.expirationDate }"
              />
              <span v-if="errors.expirationDate" class="error-message">{{ errors.expirationDate }}</span>
            </div>

            <div class="form-group">
              <label for="cardType" class="form-label">Card Type</label>
              <select
                id="cardType"
                v-model="formData.type"
                @blur="validateType"
                class="form-input"
                :class="{ 'error': errors.type }"
              >
                <option value="">Select Card Type</option>
                <option value="Visa">Visa</option>
                <option value="Mastercard">Mastercard</option>
                <option value="Amex">American Express</option>
              </select>
              <span v-if="errors.type" class="error-message">{{ errors.type }}</span>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="creditLimit" class="form-label">Credit Limit ($)</label>
              <input
                id="creditLimit"
                v-model="formData.creditLimit"
                @input="formatCreditLimit"
                @blur="validateCreditLimit"
                type="text"
                placeholder="5000.00"
                class="form-input"
                :class="{ 'error': errors.creditLimit }"
              />
              <span v-if="errors.creditLimit" class="error-message">{{ errors.creditLimit }}</span>
            </div>

            <div class="form-group">
              <label for="currentBalance" class="form-label">Current Balance ($)</label>
              <input
                id="currentBalance"
                v-model="formData.currentBalance"
                @input="formatCurrentBalance"
                @blur="validateCurrentBalance"
                type="text"
                placeholder="0.00"
                class="form-input"
                :class="{ 'error': errors.currentBalance }"
              />
              <span v-if="errors.currentBalance" class="error-message">{{ errors.currentBalance }}</span>
            </div>
          </div>


          <div class="form-actions">
            <Button
              @click="handleSubmit"
              variant="primary"
              size="large"
              :loading="isSubmitting"
              :disabled="!isFormValid"
              full-width
            >
              {{ isSubmitting ? 'Creating...' : 'Create Credit Card' }}
            </Button>
          </div>
        </form>
      </div>

      <div class="preview-section">
        <h3 class="preview-title">Card Preview</h3>
        <CreditCardPreview
          :card-type="formData.type || 'VISA'"
          :card-number="formData.cardNumber || ''"
          :cardholder-name="'CARD HOLDER'"
          :expiration-date="formatExpirationForPreview(formData.expirationDate)"
          :credit-limit="formData.creditLimit ? parseFloat(formData.creditLimit) : 0"
          :current-balance="formData.currentBalance ? parseFloat(formData.currentBalance) : 0"
          :show-balance="true"
          :show-actions="false"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, computed, ref } from 'vue';
import Button from '../common/Button.vue';
import CreditCardPreview from '../cards/CreditCardPreview.vue';
import { useCreditCardAPI } from '../../composables/useCreditCardAPI';

const emit = defineEmits(['success', 'error']);

const { generateCardNumber } = useCreditCardAPI();

const isSubmitting = ref(false);
const isGenerating = ref(false);

const formData = reactive({
  cardNumber: '',
  expirationDate: '',
  creditLimit: '',
  currentBalance: '0',
  type: ''
});

const errors = reactive({
  cardNumber: '',
  expirationDate: '',
  creditLimit: '',
  currentBalance: '',
  type: ''
});

const isFormValid = computed(() => {
  return (
    formData.cardNumber && formData.cardNumber.replace(/\s/g, '').length === 16 &&
    formData.expirationDate && formData.expirationDate.length > 0 &&
    formData.creditLimit && parseFloat(formData.creditLimit) > 0 &&
    formData.currentBalance && parseFloat(formData.currentBalance) >= 0 &&
    formData.type && ['Visa', 'Mastercard', 'Amex'].includes(formData.type) &&
    !Object.values(errors).some(error => error !== '')
  );
});

const formatCardNumber = () => {
  if (!formData.cardNumber) return;
  let value = formData.cardNumber.replace(/\s/g, '');
  let formattedValue = value.match(/.{1,4}/g)?.join(' ') || value;
  formData.cardNumber = formattedValue;
  
  // Re-validate type when card number changes
  if (formData.type) {
    validateType();
  }
};

const formatCreditLimit = () => {
  if (!formData.creditLimit) return;
  let value = formData.creditLimit.replace(/[^0-9.]/g, '');
  const parts = value.split('.');
  if (parts.length > 2) {
    value = parts[0] + '.' + parts.slice(1).join('');
  }
  if (parts[1] && parts[1].length > 2) {
    value = parts[0] + '.' + parts[1].slice(0, 2);
  }
  formData.creditLimit = value;
};

const formatCurrentBalance = () => {
  if (!formData.currentBalance) return;
  let value = formData.currentBalance.replace(/[^0-9.]/g, '');
  const parts = value.split('.');
  if (parts.length > 2) {
    value = parts[0] + '.' + parts.slice(1).join('');
  }
  if (parts[1] && parts[1].length > 2) {
    value = parts[0] + '.' + parts[1].slice(0, 2);
  }
  formData.currentBalance = value;
};

const validateCardNumber = () => {
  if (!formData.cardNumber) {
    errors.cardNumber = 'Card number is required';
    return;
  }
  const cardNumber = formData.cardNumber.replace(/\s/g, '');
  if (cardNumber.length !== 16) {
    errors.cardNumber = 'Card number must be exactly 16 digits';
  } else if (!/^\d+$/.test(cardNumber)) {
    errors.cardNumber = 'Card number must contain only digits';
  } else if (!validateLuhn(cardNumber)) {
    errors.cardNumber = 'Invalid card number (fails Luhn check)';
  } else {
    errors.cardNumber = '';
    // Auto-detect card type if not selected
    if (!formData.type) {
      formData.type = detectCardType(cardNumber);
    }
  }
};

const validateLuhn = (cardNumber) => {
  let sum = 0;
  let shouldDouble = false;
  
  // Parcourir de droite à gauche
  for (let i = cardNumber.length - 1; i >= 0; i--) {
    let digit = parseInt(cardNumber[i]);
    
    if (shouldDouble) {
      digit *= 2;
      if (digit > 9) {
        digit = Math.floor(digit / 10) + (digit % 10);
      }
    }
    
    sum += digit;
    shouldDouble = !shouldDouble;
  }
  
  return sum % 10 === 0;
};

const detectCardType = (cardNumber) => {
  if (cardNumber.startsWith('4')) {
    return 'Visa';
  } else if (cardNumber.startsWith('5') || cardNumber.startsWith('2')) {
    return 'Mastercard';
  } else if (cardNumber.startsWith('34') || cardNumber.startsWith('37')) {
    return 'Amex';
  }
  return '';
};

const validateExpirationDate = () => {
  if (!formData.expirationDate) {
    errors.expirationDate = 'Expiration date is required';
    return;
  }
  
  const [year, month] = formData.expirationDate.split('-');
  const selectedDate = new Date(parseInt(year), parseInt(month) - 1, 1);
  const now = new Date();
  const currentMonth = new Date(now.getFullYear(), now.getMonth(), 1);
  const maxDate = new Date(now.getFullYear() + 10, now.getMonth(), 1);
  
  if (selectedDate < currentMonth) {
    errors.expirationDate = 'Expiration date must be in the future';
  } else if (selectedDate > maxDate) {
    errors.expirationDate = 'Expiration date cannot be more than 10 years in the future';
  } else {
    errors.expirationDate = '';
  }
};

const validateType = () => {
  if (!formData.type) {
    errors.type = 'Card type is required';
  } else if (!['Visa', 'Mastercard', 'Amex'].includes(formData.type)) {
    errors.type = 'Card type must be Visa, Mastercard, or Amex';
  } else if (formData.cardNumber) {
    const cardNumber = formData.cardNumber.replace(/\s/g, '');
    const detectedType = detectCardType(cardNumber);
    if (detectedType && detectedType !== formData.type) {
      errors.type = `Card number doesn't match selected type. Detected: ${detectedType}`;
    } else {
      errors.type = '';
    }
  } else {
    errors.type = '';
  }
};

const validateCreditLimit = () => {
  if (!formData.creditLimit) {
    errors.creditLimit = 'Credit limit is required';
    return;
  }
  const limit = parseFloat(formData.creditLimit);
  if (isNaN(limit) || limit <= 0) {
    errors.creditLimit = 'Credit limit must be greater than 0';
  } else {
    errors.creditLimit = '';
  }
};

const validateCurrentBalance = () => {
  if (!formData.currentBalance) {
    errors.currentBalance = 'Current balance is required';
    return;
  }
  const balance = parseFloat(formData.currentBalance);
  const limit = parseFloat(formData.creditLimit);
  if (isNaN(balance) || balance < 0) {
    errors.currentBalance = 'Current balance must be 0 or greater';
  } else if (!isNaN(limit) && balance > limit) {
    errors.currentBalance = 'Current balance cannot exceed credit limit';
  } else {
    errors.currentBalance = '';
  }
};

const getCurrentMonth = () => {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
};

const getMaxMonth = () => {
  const now = new Date();
  const maxYear = now.getFullYear() + 10;
  return `${maxYear}-${String(now.getMonth() + 1).padStart(2, '0')}`;
};

const formatExpirationForPreview = (monthValue) => {
  if (!monthValue) return 'MM/YY';
  const [year, month] = monthValue.split('-');
  return `${month}/${year.slice(-2)}`;
};

const handleGenerateCardNumber = async () => {
  if (!formData.type) {
    errors.cardNumber = 'Please select a card type first';
    return;
  }

  isGenerating.value = true;
  errors.cardNumber = '';

  try {
    const result = await generateCardNumber(formData.type);
    
    if (result.success && result.cardNumber) {
      // Formater le numéro avec des espaces
      const formatted = result.cardNumber.match(/.{1,4}/g)?.join(' ') || result.cardNumber;
      formData.cardNumber = formatted;
      
      // Valider le numéro généré
      validateCardNumber();
    } else {
      errors.cardNumber = result.error || 'Failed to generate card number';
    }
  } catch (error) {
    errors.cardNumber = 'Error generating card number';
  } finally {
    isGenerating.value = false;
  }
};

const handleSubmit = async () => {
  validateCardNumber();
  validateExpirationDate();
  validateType();
  validateCreditLimit();
  validateCurrentBalance();

  if (!isFormValid.value) {
    return;
  }

  isSubmitting.value = true;

  try {
    // Convert month input (YYYY-MM) to DateTime for backend
    const [year, month] = formData.expirationDate.split('-');
    const expirationDate = new Date(parseInt(year), parseInt(month) - 1, 1);
    
    const cardData = {
      cardNumber: formData.cardNumber.replace(/\s/g, ''),
      expirationDate: expirationDate.toISOString(),
      creditLimit: parseFloat(formData.creditLimit),
      currentBalance: parseFloat(formData.currentBalance),
      type: formData.type
    };

    emit('submit', cardData);
  } catch (error) {
    emit('error', error.message);
  } finally {
    isSubmitting.value = false;
  }
};

const resetForm = () => {
  Object.assign(formData, {
    cardNumber: '',
    expirationDate: '',
    creditLimit: '',
    currentBalance: '0',
    type: ''
  });
  
  Object.assign(errors, {
    cardNumber: '',
    expirationDate: '',
    creditLimit: '',
    currentBalance: '',
    type: ''
  });
};

defineExpose({
  resetForm
});
</script>

<style scoped>
.credit-card-form {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.form-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 40px;
  align-items: start;
}

.form-section {
  background: white;
  border-radius: 12px;
  padding: 30px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.form-title {
  font-size: 24px;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 25px;
  text-align: center;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.form-row .full-width {
  grid-column: 1 / -1;
}

.input-with-button {
  display: flex;
  gap: 10px;
  align-items: stretch;
}

.input-with-button .form-input {
  flex: 1;
}

.generate-button {
  padding: 12px 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  white-space: nowrap;
  display: flex;
  align-items: center;
  gap: 6px;
}

.generate-button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.generate-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.info-message {
  font-size: 12px;
  color: #667eea;
  margin-top: 4px;
  font-weight: 500;
  display: block;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-label {
  font-size: 14px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 8px;
}

.form-input {
  padding: 12px 16px;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 15px;
  transition: all 0.3s ease;
  background: white;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-input.error {
  border-color: #e74c3c;
  box-shadow: 0 0 0 3px rgba(231, 76, 60, 0.1);
}

.error-message {
  font-size: 12px;
  color: #e74c3c;
  margin-top: 4px;
  font-weight: 500;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  font-size: 14px;
  color: #2c3e50;
  margin-top: 8px;
}

.checkbox-input {
  width: 18px;
  height: 18px;
  accent-color: #667eea;
}

.checkbox-text {
  font-weight: 500;
}

.form-actions {
  margin-top: 10px;
}

.preview-section {
  position: sticky;
  top: 20px;
}

.preview-title {
  font-size: 18px;
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 20px;
  text-align: center;
}

@media (max-width: 968px) {
  .form-container {
    grid-template-columns: 1fr;
    gap: 30px;
  }
  
  .preview-section {
    position: static;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .input-with-button {
    flex-direction: column;
  }
  
  .generate-button {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 640px) {
  .credit-card-form {
    padding: 16px;
  }
  
  .form-section {
    padding: 20px;
  }
  
  .form-title {
    font-size: 20px;
  }
}
</style>
