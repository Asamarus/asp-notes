import { Page } from '@playwright/test'
import { mainUrl } from './config'

export const login = async (page: Page, email: string, password: string) => {
  await page.goto(mainUrl)
  await page.getByRole('textbox', { name: 'Email' }).fill(email)
  await page.getByRole('textbox', { name: 'Password' }).fill(password)
  await page.getByRole('button', { name: 'Login' }).press('Enter')
  await page.waitForSelector('button[aria-label="User account"]')
}

export const addNewSection = async (
  page: Page,
  name: string,
  displayName: string,
  color: string,
) => {
  await page.goto(`${mainUrl}settings/sections`)

  // Check if the section already exists
  const sectionExists = await page
    .waitForSelector(`text=${displayName}`, { state: 'visible', timeout: 5000 })
    .catch(() => null)
  if (sectionExists) {
    console.log(`Section with display name "${displayName}" already exists.`)
    return
  }

  await page.getByRole('button', { name: 'Add new section' }).click()
  await page.getByRole('textbox', { name: 'Name', exact: true }).fill(name)
  await page.getByRole('textbox', { name: 'Display name' }).fill(displayName)
  await page.getByRole('textbox', { name: 'Color' }).fill(color)
  await page.getByRole('button', { name: 'Add', exact: true }).click()
  await page.waitForSelector('text=Section is created!')
}

export const deleteNote = async (page: Page) => {
  await page.waitForTimeout(1000)
  await page.getByRole('dialog').press('Tab')
  await page.keyboard.press('Space')
  await page.getByRole('menuitem', { name: 'Delete' }).click()
  await page.getByRole('button', { name: 'Delete note' }).click()
  await page.getByRole('button', { name: 'Confirm' }).click()
  await page.waitForSelector('text=Note is deleted!')
}

export const deleteNoteInList = async (page: Page) => {
  await page.locator('button[aria-haspopup="menu"]').nth(1).click()
  await page.getByRole('menuitem', { name: 'Delete' }).click()
  await page.getByRole('button', { name: 'Delete note' }).click()
  await page.getByRole('button', { name: 'Confirm' }).click()
  await page.waitForSelector('text=Note is deleted!')
}

export const getCurrentDay = (): string => {
  const date = new Date()
  return String(date.getDate()).padStart(2, '0')
}

export const getCurrentMonth = (): string => {
  const date = new Date()
  const options: Intl.DateTimeFormatOptions = { month: 'long' }
  return date.toLocaleDateString('en-US', options)
}

export const getCurrentDateFormatted = () => {
  const date = new Date()
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}
