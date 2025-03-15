import { test, expect } from '@playwright/test'
import { login } from '../common/utils'
import { userCredentials } from '../common/config'

test.describe('User Actions', () => {
  test.beforeEach(async ({ page }) => {
    await login(page, userCredentials.email, userCredentials.password)
  })

  test('can log in', async ({ page }) => {
    await expect(page.getByRole('button', { name: 'User account' })).toBeVisible()
  })

  test('can change password', async ({ page }) => {
    await page.getByRole('button', { name: 'User account' }).click()
    await page.getByRole('menuitem', { name: 'Change password' }).click()
    await page.getByRole('textbox', { name: 'Current password' }).fill(userCredentials.password)
    await page
      .getByRole('textbox', { name: 'New password', exact: true })
      .fill(userCredentials.newPassword)
    await page
      .getByRole('textbox', { name: 'New password confirmation' })
      .fill(userCredentials.newPassword)
    await page.getByRole('button', { name: 'Update password' }).press('Enter')

    await expect(page.locator('text=Password is updated!')).toBeVisible()
    await page.getByRole('button', { name: 'User account' }).click()
    await page.getByRole('menuitem', { name: 'Logout' }).click()

    await login(page, 'user@mail.com', userCredentials.newPassword)
    await page.getByRole('button', { name: 'User account' }).click()
    await page.getByRole('menuitem', { name: 'Change password' }).click()
    await page.getByRole('textbox', { name: 'Current password' }).fill(userCredentials.password)
    await page
      .getByRole('textbox', { name: 'New password', exact: true })
      .fill(userCredentials.password)
    await page
      .getByRole('textbox', { name: 'New password confirmation' })
      .fill(userCredentials.password)
    await page.getByRole('button', { name: 'Update password' }).press('Enter')

    await expect(page.locator('text=Invalid current password!')).toBeVisible()
    await page.getByRole('textbox', { name: 'Current password' }).fill(userCredentials.newPassword)
    await page.getByRole('button', { name: 'Update password' }).press('Enter')
    await expect(page.locator('text=Password is updated!')).toBeVisible()
  })

  test('change password form validation', async ({ page }) => {
    await expect(page.getByRole('button', { name: 'User account' })).toBeVisible()
    await page.getByRole('button', { name: 'User account' }).click()
    await page.getByRole('menuitem', { name: 'Change password' }).click()
    await page.getByRole('button', { name: 'Update password' }).click()

    const errorMessage = 'Minimum password length is 6'
    const errorMessages = page.locator(`text=${errorMessage}`)
    await expect(errorMessages).toHaveCount(3)

    await page.getByRole('textbox', { name: 'Current password' }).click()
    await page.getByRole('textbox', { name: 'New password', exact: true }).click()
    await page.getByRole('textbox', { name: 'New password', exact: true }).fill('1234567')
    await page.getByRole('textbox', { name: 'New password confirmation' }).click()
    await page.getByRole('textbox', { name: 'New password confirmation' }).fill('123456')
    await page.getByRole('button', { name: 'Update password' }).click()

    await expect(page.locator('text=Passwords do not match')).toBeVisible()
  })
})
