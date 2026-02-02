<template>
  <div class="edit-credit-card-form">
    <div class="form-container">
      <div class="form-section">
        <h2 class="form-title">Update Credit Card Details</h2>
        
        <form class="form" @submit.prevent>
          <!-- Read-only Fields -->
          <div class="form-row">
            <div class="form-group">
              <label for="cardNumber" class="form-label">Card Number (Read-only)</label>
              <input
                id="cardNumber"
                :value="formatCardNumberDisplay(card.cardNumber)"
                type="text"
                class="form-input read-only"
                disabled
              />
            </div>

            <div class="form-group">
              <label for="cardType" class="form-label">Card Type (Read-only)</label>
              <input
                id="cardType"
                :value="card.creditCardType || 'N/A'"
                type="text"
                class="form-input read-only"
                disabled
              />
            </div>
          </div>

          <!-- Editable Fields -->
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

          <!-- Additional Info -->
          <div class="info-section">
            <div class="info-item">
              <span class="info-label">Expiration Date:</span>
              <span class="info-value">{{ formatExpirationDate(card.dateExpiration) }}</span>
            </div>
            <div class="info-item">
              <span class="info-label">Status:</span>
              <span class="info-value" :class="{ 'active': card.isActive, 'inactive': !card.isActive }">
                {{ card.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>

          <div class="form-actions">
            <Button
              @click="handleCancel"
              variant="secondary"
              size="large"
              full-width
            >
              Cancel
            </Button>
            <Button
              @click="handleSubmit"
              variant="primary"
              size="large"
              :loading="isSubmitting"
              :disabled="!isFormValid"
              full-width
            >
              {{ isSubmitting ? 'Updating...' : 'Update Credit Card' }}
            </Button>
          </div>
        </form>
      </div>

      <div class="preview-section">
        <h3 class="preview-title">Card Preview</h3>
        <CreditCardPreview
          :card-type="card.creditCardType || 'VISA'"
          :card-number="card.cardNumber || ''"
          :cardholder-name="'CARD HOLDER'"
          :expiration-date="formatExpirationDate(card.dateExpiration)"
          :credit-limit="parseFloat(formData.creditLimit) || 0"
          :current-balance="parseFloat(formData.currentBalance) || 0"
          :show-balance="true"
          :show-actions="false"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, computed, ref, watch } from 'vue';
import Button from '../common/Button.vue';
import CreditCardPreview from '../cards/CreditCardPreview.vue';

const props = defineProps({
  card: {
    type: Object,
    required: true
  }
});

const emit = defineEmits(['submit', 'error', 'cancel']);

const isSubmitting = ref(false);

const formData = reactive({
  creditLimit: '',
  currentBalance: ''
});

const errors = reactive({
  creditLimit: '',
  currentBalance: ''
});

// Initialize form data when card prop changes
watch(() => props.card, (newCard) => {
  if (newCard) {
    formData.creditLimit = newCard.creditLimit?.toString() || '';
    formData.currentBalance = newCard.currentBalance?.toString() || '';
  }
}, { immediate: true });

const isFormValid = computed(() => {
  const creditLimit = parseFloat(formData.creditLimit);
  const currentBalance = parseFloat(formData.currentBalance);
  
  return (
    formData.creditLimit &&
    !isNaN(creditLimit) &&
    creditLimit > 0 &&
    formData.currentBalance !== undefined &&
    !isNaN(currentBalance) &&
    currentBalance >= 0 &&
    currentBalance <= creditLimit &&
    !errors.creditLimit &&
    !errors.currentBalance
  );
});

const formatCardNumberDisplay = (cardNumber) => {
  if (!cardNumber) return '';
  const str = cardNumber.toString();
  return str.match(/.{1,4}/g)?.join(' ') || str;
};

const formatExpirationDate = (dateString) => {
  if (!dateString) return 'N/A';
  try {
    const date = new Date(dateString);
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = String(date.getFullYear()).slice(-2);
    return `${month}/${year}`;
  } catch {
    return 'N/A';
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
  if (formData.currentBalance === undefined || formData.currentBalance === '') {
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

const handleSubmit = async () => {
  validateCreditLimit();
  validateCurrentBalance();

  if (!isFormValid.value) {
    return;
  }

  isSubmitting.value = true;

  try {
    const payload = {
      creditLimit: parseFloat(formData.creditLimit),
      currentBalance: parseFloat(formData.currentBalance)
    };

    emit('submit', payload);
  } catch (error) {
    emit('error', error.message);
  } finally {
    isSubmitting.value = false;
  }
};

const handleCancel = () => {
  emit('cancel');
};
</script>

<style scoped>
.edit-credit-card-form {
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

.form-input.read-only {
  background-color: #f8f9fa;
  color: #6c757d;
  cursor: not-allowed;
}

.form-input.read-only:focus {
  border-color: #e9ecef;
  box-shadow: none;
}

.error-message {
  font-size: 12px;
  color: #e74c3c;
  margin-top: 4px;
  font-weight: 500;
}

.info-section {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.info-label {
  font-size: 14px;
  font-weight: 600;
  color: #2c3e50;
}

.info-value {
  font-size: 14px;
  color: #6c757d;
  font-weight: 500;
}

.info-value.active {
  color: #28a745;
  font-weight: 600;
}

.info-value.inactive {
  color: #dc3545;
  font-weight: 600;
}

.form-actions {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
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
  
  .form-actions {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .edit-credit-card-form {
    padding: 16px;
  }
  
  .form-section {
    padding: 20px;
  }
  
  .form-title {
    font-size: 20px;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 16px;
  }
}
</style>
